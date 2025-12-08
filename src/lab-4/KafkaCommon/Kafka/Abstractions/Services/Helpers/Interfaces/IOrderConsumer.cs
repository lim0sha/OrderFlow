using Confluent.Kafka;
using Orders.Kafka.Contracts;
using System.Threading.Channels;

namespace Kafka.Abstractions.Services.Helpers.Interfaces;

public interface IOrderConsumer
{
    Task ConsumeAsync(ChannelWriter<ConsumeResult<long, OrderProcessingValue>> writer,
        CancellationToken token);
}