using Amazon.S3;
using DocumentService.Application.Features.Commands.Document.UploadDocument;
using DocumentService.DependencyInjection;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Security.Claims;
using System.Text;
using DocumentService.Date;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//MediatR
builder.Services.AddValidatorsFromAssembly(typeof(UploadDocumentValidator).Assembly);
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(UploadDocumentValidator).Assembly);
});

//EF Core + Dapper + Postgres + S3
builder.Services.AddInfrastructure(builder.Configuration);

//Authentification
var secretKey = builder.Configuration["JwtSettings:SecretKey"]
    ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured in appsettings or environment variables.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            NameClaimType = ClaimTypes.NameIdentifier
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (string.IsNullOrEmpty(context.Token) && context.Request.Cookies.TryGetValue("accessToken", out var token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// Явная регистрация SwaggerDoc "v1"
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Document Service API",
        Version = "v1"
    });
});

builder.Services.AddHttpLogging();


var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<DocumentDbContext>();

        dbContext.Database.Migrate();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Document Service API v1");
    });
}

app.UseHttpLogging();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var s3Client = scope.ServiceProvider.GetRequiredService<IAmazonS3>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var bucketName = config["S3Settings:BucketName"] ?? "documents";

    var response = await s3Client.ListBucketsAsync();
    var bucketExists = response.Buckets?.Any(b => b.BucketName == bucketName) ?? false;

    if (!bucketExists)
    {
        await s3Client.PutBucketAsync(bucketName);
    }
}

app.Run();
