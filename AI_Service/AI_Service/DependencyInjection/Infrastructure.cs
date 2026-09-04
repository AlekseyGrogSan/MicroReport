using AI_Service.Core.Interfaces;
using AI_Service.Core.Models;
using AI_Service.Core.Settings;
using AI_Service.Infrastructure;
using Amazon.S3;
using Microsoft.SemanticKernel;
using System.Threading.Channels;

namespace AI_Service.DependencyInjection
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

            services.AddSingleton(Channel.CreateBounded<RequestModel>(new BoundedChannelOptions(100)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = true
            }));

            services.Configure<KafkaSettings>(config.GetSection(KafkaSettings.SectionName));
            services.Configure<OllamaSettings>(config.GetSection("OllamaSettings"));

            var ollama = config.GetSection("OllamaSettings").Get<OllamaSettings>()!;
            if (ollama != null) 
            {
                var kernekBuilder = Kernel.CreateBuilder();
                kernekBuilder.AddOllamaChatCompletion(
                    modelId: ollama.ModelId,
                    endpoint: new Uri(ollama.Endpoint)
                    );
                services.AddSingleton(kernekBuilder.Build());
            }

            services.AddScoped<IDocumentParserService, DocumentParserService>();
            services.AddScoped<IReportExporterService, ReportExporterService>();
            services.AddScoped<IReportExporterService, ReportExporterService>();
            services.AddScoped<IS3Service, S3Service>();

            return services;
        }
    }
}
