using Confluent.Kafka;
using Kafka.Abstractions.Interfaces;
using Kafka.Abstractions.Options;
using Kafka.Abstractions.Services.Helpers.Interfaces;
using Microsoft.Extensions.Options;
using Orders.Kafka.Contracts;
using System.Threading.Channels;

namespace Kafka.Abstractions.Services.Helpers;

public class OrderProcessor : IOrderProcessor
{
    private readonly IKafkaMessageHandler<long, OrderProcessingValue> _handler;
    private readonly IOptions<KafkaOptions> _options;

    public OrderProcessor(
        IKafkaMessageHandler<long, OrderProcessingValue> handler,
        IOptions<KafkaOptions> options)
    {
        _handler = handler;
        _options = options;
    }

    public async Task ProcessAsync(
        ChannelReader<ConsumeResult<long, OrderProcessingValue>> reader,
        CancellationToken token)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _options.Value.BootstrapServers,
        };

        using IConsumer<long, OrderProcessingValue> consumer = new ConsumerBuilder<long, OrderProcessingValue>(config).Build();

        await foreach (ConsumeResult<long, OrderProcessingValue> msg in reader.ReadAllAsync(token))
        {
            try
            {
                await _handler.HandleAsync(msg.Message.Key, msg.Message.Value, token);
                consumer.Commit(msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Handler Error] {ex}");
            }
        }
    }
}
