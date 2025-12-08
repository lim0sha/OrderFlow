namespace Kafka.Abstractions.Interfaces;

public interface IOrderPublisher
{
    Task PublishOrderCreatedAsync(long orderId, CancellationToken ct);

    Task PublishOrderProcessingStartedAsync(long orderId, CancellationToken ct);
}