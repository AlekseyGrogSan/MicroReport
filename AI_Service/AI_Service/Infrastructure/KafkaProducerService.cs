using AI_Service.Core.Interfaces;
using AI_Service.Core.Settings;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AI_Service.Infrastructure
{
    public class KafkaProducerService : IDisposable, IKafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;

        public KafkaProducerService(IOptions<KafkaSettings> settings)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = settings.Value.BootstrapServers,
                Acks = Acks.All
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task ProduceAsync<T>(string topic, T message, CancellationToken cancellationToken)
        {
            var jsonMessage = JsonSerializer.Serialize(message);

            await _producer.ProduceAsync(topic, new Message<Null, string>
            {
                Value = jsonMessage
            }, cancellationToken);
        }

        public void Dispose()
        {
            _producer?.Flush(TimeSpan.FromSeconds(5));
            _producer?.Dispose();
        }
    }
}
