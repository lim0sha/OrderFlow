using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Orders.Kafka.Contracts;
using Presentation.Kafka.Abstractions.Interfaces;
using Presentation.Kafka.Abstractions.Options;
using Presentation.Kafka.Serializers;

namespace Presentation.Kafka.Abstractions.Services;

public class OrderProcessingKafkaConsumer : BackgroundService
{
    private readonly IOptions<KafkaOptions> _options;
    private readonly IKafkaMessageHandler<long, OrderProcessingValue> _handler;

    public OrderProcessingKafkaConsumer(
        IOptions<KafkaOptions> options,
        IKafkaMessageHandler<long, OrderProcessingValue> handler)
    {
        _options = options;
        _handler = handler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _options.Value.BootstrapServers,
            GroupId = $"{_options.Value.OrderProcessingTopic}-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

        using IConsumer<long, OrderProcessingValue> consumer = new ConsumerBuilder<long, OrderProcessingValue>(config)
            .SetValueDeserializer(new ProtobufDeserializer<OrderProcessingValue>())
            .SetKeyDeserializer(Deserializers.Int64)
            .Build();

        consumer.Subscribe(_options.Value.OrderProcessingTopic);

        int batchSize = _options.Value.BatchSize;
        var pollTimeout = TimeSpan.FromMilliseconds(_options.Value.PollTimeoutMs);

        while (!stoppingToken.IsCancellationRequested)
        {
            var messages = new List<ConsumeResult<long, OrderProcessingValue>>();
            for (int i = 0; i < batchSize && !stoppingToken.IsCancellationRequested; i++)
            {
                ConsumeResult<long, OrderProcessingValue> result = consumer.Consume(pollTimeout);
                if (result?.Message != null)
                    messages.Add(result);
                else
                    break;
            }

            if (messages.Count == 0) continue;

            try
            {
                foreach (ConsumeResult<long, OrderProcessingValue> msg in messages)
                {
                    await _handler.HandleAsync(msg.Message.Key, msg.Message.Value, stoppingToken);
                }

                consumer.Commit(messages.Last());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Kafka Consumer Error] {ex}");
            }
        }
    }
}