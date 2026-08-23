using DocumentService.Infrastructure;
using Amazon.S3;
using DocumentService.Core.Interfaces;
using DocumentService.Core.Settings;

namespace DocumentService.DependencyInjection
{
    public static class Infrastructure
    {
        public static IServiceCollection AddInfrastructure(this  IServiceCollection services, IConfiguration config)
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

            services.AddScoped<IFileStorageService, FileStorageServise>();

            return services;
        }
    }
}
