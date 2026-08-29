using Confluent.Kafka;
using DocumentService.Application.Interfaces;
using DocumentService.Core.DTOs;
using DocumentService.Core.Entities;
using DocumentService.Core.Settings;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace DocumentService.Infrastructure
{
    public class ReportGenerationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IOptionsMonitor<KafkaSettings> _kafkaSettings;
        private readonly ILogger<ReportGenerationBackgroundService> _logger;

        public ReportGenerationBackgroundService(
            IServiceScopeFactory scopeFactory,
            IOptionsMonitor<KafkaSettings> kafkaSettings,
            ILogger<ReportGenerationBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _kafkaSettings = kafkaSettings;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            var settings = _kafkaSettings.CurrentValue;
            var topic = settings.Topics["ReportCompleted"];

            var config = new ConsumerConfig
            {
                BootstrapServers = settings.BootstrapServers,
                GroupId = settings.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build(); 
            consumer.Subscribe(topic);

            _logger.LogInformation("Kafka Consumer succesful started and listen topic {topicName}", topic);

            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumerResult = consumer.Consume(stoppingToken);
                    if (consumerResult == null ||
                        string.IsNullOrEmpty(consumerResult.Message.Value))
                    {
                        continue;
                    }

                    var completedEvent = JsonSerializer.Deserialize<ReportCompletedEventDto>(consumerResult.Message.Value);
                    if (completedEvent == null)
                    {
                        _logger.LogWarning("Не удалось десериализовать событие завершения отчета.");
                        consumer.Commit(consumerResult);
                        continue;
                    }

                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var reportRepository = scope.ServiceProvider.GetRequiredService<IReportRepository>();
                        var reportRequestRepository = scope.ServiceProvider.GetRequiredService<IReportRequestRepository>();

                        var report = Report.Create(
                            requestId: completedEvent.RequestId,
                            userId: completedEvent.UserId,
                            reportName: completedEvent.ReportName,
                            contentType: completedEvent.ContentType,
                            s3Key: completedEvent.S3Key
                        );

                        var reportRequest = await reportRequestRepository.GetByIdAsync(completedEvent.RequestId, stoppingToken);
                        if (reportRequest != null)
                        {
                            reportRequest.MarkAsCompleted(); 
                        }

                        await reportRepository.AddReportAsync(report, stoppingToken);
                        await reportRequestRepository.SaveChangesAsync(stoppingToken);
                    }
                }
                catch
                {

                }
            }
        }
    }
}
