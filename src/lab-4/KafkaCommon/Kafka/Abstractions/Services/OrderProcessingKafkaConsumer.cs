using Confluent.Kafka;
using Kafka.Abstractions.Interfaces;
using Kafka.Abstractions.Options;
using Kafka.Serializers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Orders.Kafka.Contracts;
using System.Threading.Channels;

namespace Kafka.Abstractions.Services;

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
            EnableAutoCommit = false,
            EnableAutoOffsetStore = false,
        };

        using IConsumer<long, OrderProcessingValue> consumer = new ConsumerBuilder<long, OrderProcessingValue>(config)
            .SetValueDeserializer(new ProtobufDeserializer<OrderProcessingValue>())
            .SetKeyDeserializer(Deserializers.Int64)
            .Build();

        consumer.Subscribe(_options.Value.OrderProcessingTopic);

        int batchSize = _options.Value.BatchSize;
        var pollTimeout = TimeSpan.FromMilliseconds(_options.Value.PollTimeoutMs);

        var channel = Channel.CreateUnbounded<ConsumeResult<long, OrderProcessingValue>>(
            new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });

        var readerTask = Task.Run(
            async () =>
            {
                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var messages = new List<ConsumeResult<long, OrderProcessingValue>>();
                        for (int i = 0; i < batchSize && !stoppingToken.IsCancellationRequested; i++)
                        {
                            ConsumeResult<long, OrderProcessingValue> result = consumer.Consume(pollTimeout);
                            if (result?.Message != null)
                            {
                                messages.Add(result);
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (messages.Count == 0)
                        {
                            await Task.Delay(10, stoppingToken);
                            continue;
                        }

                        foreach (ConsumeResult<long, OrderProcessingValue> msg in messages)
                        {
                            if (stoppingToken.IsCancellationRequested) break;
                            await channel.Writer.WriteAsync(msg, stoppingToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Kafka Reader Error] {ex}");
                    channel.Writer.Complete(ex);
                }
                finally
                {
                    channel.Writer.Complete();
                }
            },
            stoppingToken);

        var processorTask = Task.Run(
            async () =>
            {
                try
                {
                    await foreach (ConsumeResult<long, OrderProcessingValue> msg in channel.Reader.ReadAllAsync(
                                       stoppingToken))
                    {
                        try
                        {
                            await _handler.HandleAsync(msg.Message.Key, msg.Message.Value, stoppingToken);
                            consumer.Commit(msg);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[Message Handler Error] Key={msg.Message.Key}, Error={ex}");
                        }
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    Console.WriteLine($"[Kafka Processor Error] {ex}");
                }
            },
            stoppingToken);

        await Task.WhenAny(readerTask, processorTask, Task.Delay(Timeout.Infinite, stoppingToken));
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        cts.CancelAfter(TimeSpan.FromSeconds(10));
        await Task.WhenAll(readerTask, processorTask).WaitAsync(cts.Token);
    }
}