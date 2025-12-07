namespace Presentation.Kafka.Abstractions.Interfaces;

public interface IKafkaMessageHandler<TKey, TValue>
{
    Task HandleAsync(TKey key, TValue value, CancellationToken ct);
}