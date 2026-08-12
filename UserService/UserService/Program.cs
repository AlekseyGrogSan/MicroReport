using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using UserService.Core.Interfaces;
using UserService.Core.Settings;
using UserService.Data;
using UserService.Data.Repositories;
using UserService.Infrastructure;
using UserService.Services;

var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection(JWTSettings.SectionName));

var jwtSettings = builder.Configuration.GetSection(JWTSettings.SectionName).Get<JWTSettings>()!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}
)
 .AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = jwtSettings.Issuer,
         ValidAudience = jwtSettings.Audience,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
         ClockSkew = TimeSpan.Zero
     };

     options.Events = new JwtBearerEvents
     {
         OnMessageReceived = context =>
         {
             if (context.Request.Cookies.TryGetValue("accessToken", out var res))
             {
                 context.Token = res;
             }
             return Task.CompletedTask;
         }
     };
 });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped<IUserService, UsersService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUserRepository, UserRepository>(); 


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "User API v1");
        c.RoutePrefix = string.Empty; // Теперь Swagger откроется прямо по http://localhost:5000/
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
