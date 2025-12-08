using Confluent.Kafka;
using Kafka.Abstractions.Options;
using Kafka.Abstractions.Services.Helpers.Interfaces;
using Kafka.Serializers;
using Microsoft.Extensions.Options;
using Orders.Kafka.Contracts;
using System.Threading.Channels;

namespace Kafka.Abstractions.Services.Helpers;

public class OrderConsumer : IOrderConsumer
{
    private readonly IOptions<KafkaOptions> _options;

    public OrderConsumer(IOptions<KafkaOptions> options)
    {
        _options = options;
    }

    public async Task ConsumeAsync(
        ChannelWriter<ConsumeResult<long, OrderProcessingValue>> writer,
        CancellationToken token)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _options.Value.BootstrapServers,
            GroupId = $"{_options.Value.OrderProcessingTopic}-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            EnableAutoOffsetStore = false,
        };

        using IConsumer<long, OrderProcessingValue> consumer = new ConsumerBuilder<long, OrderProcessingValue>(config)
            .SetValueDeserializer(new ProtobufDeserializer<OrderProcessingValue>())
            .SetKeyDeserializer(Deserializers.Int64)
            .Build();

        consumer.Subscribe(_options.Value.OrderProcessingTopic);

        int batchSize = _options.Value.BatchSize;
        var timeout = TimeSpan.FromMilliseconds(_options.Value.PollTimeoutMs);

        try
        {
            while (!token.IsCancellationRequested)
            {
                var batch = new List<ConsumeResult<long, OrderProcessingValue>>();

                for (int i = 0; i < batchSize && !token.IsCancellationRequested; i++)
                {
                    ConsumeResult<long, OrderProcessingValue> result = consumer.Consume(timeout);
                    if (result?.Message != null)
                        batch.Add(result);
                    else
                        break;
                }

                if (batch.Count == 0)
                {
                    await Task.Delay(10, token);
                    continue;
                }

                foreach (ConsumeResult<long, OrderProcessingValue> msg in batch)
                    await writer.WriteAsync(msg, token);
            }
        }
        finally
        {
            writer.Complete();
        }
    }
}
