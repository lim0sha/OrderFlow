namespace Presentation.Kafka.Abstractions.Interfaces;

public interface IKafkaMessageHandler<in TKey, in TValue>
{
    Task HandleAsync(TKey key, TValue value, CancellationToken ct);
}