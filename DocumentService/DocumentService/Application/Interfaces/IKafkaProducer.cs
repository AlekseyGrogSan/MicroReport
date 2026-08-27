namespace DocumentService.Application.Interfaces
{
    public interface IKafkaProducer
    {
        Task ProduceAsync<T>(string topic, T message, CancellationToken token);
    }
}