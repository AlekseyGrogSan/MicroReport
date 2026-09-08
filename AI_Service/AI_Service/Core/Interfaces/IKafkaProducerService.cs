namespace AI_Service.Core.Interfaces
{
    public interface IKafkaProducerService
    {
        void Dispose();
        Task ProduceAsync<T>(string topic, T message, CancellationToken cancellationToken);
    }
}