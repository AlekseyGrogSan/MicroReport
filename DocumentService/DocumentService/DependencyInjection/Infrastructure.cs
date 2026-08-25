using DocumentService.Infrastructure;
using Amazon.S3;
using DocumentService.Core.Settings;
using DocumentService.Date;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Npgsql;
using DocumentService.Application.Interfaces;
using DocumentService.Date.Repositories;

namespace DocumentService.DependencyInjection
{
    public static class Infrastructure
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            var s3Settings = config.GetSection(S3Settings.SectionName).Get<S3Settings>()!;

            services.Configure<S3Settings>(config.GetSection(S3Settings.SectionName));

            services.AddSingleton<IAmazonS3>(_ =>
            {
                var configure = new AmazonS3Config
                {
                    ServiceURL = s3Settings.Endpoint,
                    ForcePathStyle = true
                };

                return new AmazonS3Client(s3Settings.AccessKey, s3Settings.SecretKey, configure);
            });

            // Безопасный фолбэк для строки подключения к Postgres
            var connectionString = config.GetConnectionString("Postgres")
                                   ?? config.GetConnectionString("DefaultConnection")
                                   ?? config["ConnectionStrings:Postgres"]
                                   ?? config["ConnectionStrings__Postgres"];

            services.AddDbContext<DocumentDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddTransient<IDbConnection>((sp) =>
                new NpgsqlConnection(connectionString));

            services.AddScoped<IFileStorageService, FileStorageServise>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IDocumentReadRepository, DocumentReadRepository>();

            return services;
        }
    }
}
