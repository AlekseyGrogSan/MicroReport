using Microsoft.OpenApi;


var builder = WebApplication.CreateBuilder(args);

// Подключаем YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var scheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите JWT токен для всех сервисов"
    };

    options.AddSecurityDefinition("Bearer", scheme);

    // В Microsoft.OpenApi v2 передаем лямбду (doc => requirement)
    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", doc),
            new List<string>()
        }
    });
});

var app = builder.Build();

// Настраиваем единый Swagger UI с несколькими эндпоинтами
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/user-service/v1/swagger.json", "User Service API");
    options.SwaggerEndpoint("/swagger/doc-service/v1/swagger.json", "Document Service API");
    options.RoutePrefix = "swagger"; 
});

app.MapReverseProxy();

app.Run();
