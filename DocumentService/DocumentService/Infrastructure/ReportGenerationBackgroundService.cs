using Confluent.Kafka;
using DocumentService.Application.Interfaces;
using DocumentService.Core.DTOs;
using DocumentService.Core.Entities;
using DocumentService.Core.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

            if (!settings.Topics.TryGetValue("ReportCompleted", out var topic) || string.IsNullOrEmpty(topic))
            {
                _logger.LogError("Топик 'ReportCompleted' не найден в конфигурации KafkaSettings.");
                return;
            }

            var config = new ConsumerConfig
            {
                BootstrapServers = settings.BootstrapServers,
                GroupId = settings.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false 
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe(topic);

            _logger.LogInformation("Kafka Consumer успешно запущен и слушает топик {TopicName}", topic);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumerResult = consumer.Consume(stoppingToken);
                        if (consumerResult == null || string.IsNullOrEmpty(consumerResult.Message.Value))
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

                        // 4. Подтверждаем обработку в Kafka строго ПОСЛЕ сохранения в БД
                        consumer.Commit(consumerResult);
                        _logger.LogInformation("Отчет по запросу {RequestId} успешно сохранен в БД.", completedEvent.RequestId);
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Ошибка при получении сообщения из Kafka.");
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Ошибка десериализации сообщения из Kafka.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ошибка при обработке результата генерации отчета.");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Остановка Kafka Consumer по сигналу CancellationToken.");
            }
            finally
            {
                // Обязательно отписываемся и закрываем сокет при выходе
                consumer.Close();
            }
        }
    }
}