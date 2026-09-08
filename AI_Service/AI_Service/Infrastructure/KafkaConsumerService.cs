using AI_Service.Core.DTOs;
using AI_Service.Core.Models;
using AI_Service.Core.Settings;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Threading.Channels;

namespace AI_Service.Infrastructure
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly ChannelWriter<RequestModel> _channelWriter;
        private readonly IOptionsMonitor<KafkaSettings> _kafkaSettings;
        private readonly ILogger<KafkaConsumerService> _logger;

        public KafkaConsumerService(
            Channel<RequestModel> channel,
            IOptionsMonitor<KafkaSettings> kafkaSettings,
            ILogger<KafkaConsumerService> logger)
        {
            _channelWriter = channel.Writer;
            _kafkaSettings = kafkaSettings;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            var settings = _kafkaSettings.CurrentValue;

            if (!settings.Topics.TryGetValue("ReportRequested", out var topic) || string.IsNullOrEmpty(topic))
            {
                _logger.LogError("Топик 'ReportRequested' не найден в конфигурации KafkaSettings.");
                return;
            }

            var config = new ConsumerConfig
            {
                BootstrapServers = settings.BootstrapServers,
                GroupId = settings.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                SocketTimeoutMs = 10000,
                SessionTimeoutMs = 10000,
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();

            _logger.LogInformation("Подключение к Kafka брокеру {BootstrapServers}...", settings.BootstrapServers);
            consumer.Subscribe(topic);
            _logger.LogInformation("Kafka Consumer успешно запущен и слушает топик {TopicName}", topic);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumerResult = consumer.Consume(stoppingToken);

                        if (consumerResult?.Message?.Value == null)
                            continue;

                        var request = JsonSerializer.Deserialize<RequestModel>(consumerResult.Message.Value);

                        if (request != null)
                        {
                            await _channelWriter.WriteAsync(request, stoppingToken);

                            _logger.LogInformation("Запрос {RequestId} успешно записан в Bounded Channel.", request.RequestId);
                        }

                        consumer.Commit(consumerResult);
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
