using Confluent.Kafka;
using DocumentService.Application.Interfaces;
using DocumentService.Core.Settings;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace DocumentService.Infrastructure
{
    public class KafkaProducer: IKafkaProducer
    {
        private IProducer<string, string> _producer;
        private IOptionsMonitor<KafkaSettings> _settings;

        public KafkaProducer(IOptionsMonitor<KafkaSettings> settings)
        {
            _settings = settings;
            _producer = BuildProducer(_settings.CurrentValue);

            _settings.OnChange(newSettings =>
            {
                var oldProducer = _producer;
                _producer = BuildProducer(newSettings);
                oldProducer?.Dispose();
            });
        }

        private static IProducer<string, string> BuildProducer(KafkaSettings settings)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = settings.BootstrapServers,
                Acks = Acks.All,
                EnableIdempotence = true
            };

            return new ProducerBuilder<string, string>(config).Build();
        }

        public async Task ProduceAsync<T>(string topic, T message, CancellationToken token)
        {
            var json = JsonSerializer.Serialize(message);
            await _producer.ProduceAsync(topic, new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = json
            }, token);
        }
    }
}
