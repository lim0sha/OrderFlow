namespace Presentation.Kafka.Abstractions.Interfaces;

public interface IKafkaProducer<TKey, TValue>
{
    Task ProduceAsync(TKey key, TValue value, CancellationToken ct = default);
}