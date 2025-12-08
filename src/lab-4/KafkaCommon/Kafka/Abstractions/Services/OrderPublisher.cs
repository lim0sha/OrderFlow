using Kafka.Abstractions.Interfaces;
using Orders.Kafka.Contracts;
using Timestamp = Google.Protobuf.WellKnownTypes.Timestamp;

namespace Kafka.Abstractions.Services;

public class OrderPublisher : IOrderPublisher
{
    private readonly IKafkaProducer<long, OrderCreationValue> _kafkaProducer;

    public OrderPublisher(IKafkaProducer<long, OrderCreationValue> kafkaProducer)
    {
        _kafkaProducer = kafkaProducer;
    }

    public async Task PublishOrderCreatedAsync(long orderId, CancellationToken ct)
    {
        await _kafkaProducer.ProduceAsync(
            orderId,
            new OrderCreationValue
            {
                OrderCreated = new OrderCreationValue.Types.OrderCreated
                {
                    OrderId = orderId,
                    CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
                },
            },
            ct);
    }

    public async Task PublishOrderProcessingStartedAsync(long orderId, CancellationToken ct)
    {
        await _kafkaProducer.ProduceAsync(
            orderId,
            new OrderCreationValue
            {
                OrderProcessingStarted = new OrderCreationValue.Types.OrderProcessingStarted
                {
                    OrderId = orderId,
                    StartedAt = Timestamp.FromDateTime(DateTime.UtcNow),
                },
            },
            ct);
    }
}