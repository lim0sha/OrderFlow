using Confluent.Kafka;
using Orders.Kafka.Contracts;
using System.Threading.Channels;

namespace Kafka.Abstractions.Services.Helpers.Interfaces;

public interface IOrderProcessor
{
    Task ProcessAsync(
        ChannelReader<ConsumeResult<long, OrderProcessingValue>> reader,
        CancellationToken token);
}