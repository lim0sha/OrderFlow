using Confluent.Kafka;
using Kafka.Abstractions.Services.Helpers.Interfaces;
using Microsoft.Extensions.Hosting;
using Orders.Kafka.Contracts;
using System.Threading.Channels;

namespace Kafka.Abstractions.Services;

public class KafkaConsumer : BackgroundService
{
    private readonly IOrderConsumer _reader;
    private readonly IOrderProcessor _processor;

    public KafkaConsumer(
        IOrderConsumer reader,
        IOrderProcessor processor)
    {
        _reader = reader;
        _processor = processor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = Channel.CreateUnbounded<
            ConsumeResult<long, OrderProcessingValue>>(
            new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });

        await Task.Yield();
        Task readTask = _reader.ConsumeAsync(channel.Writer, stoppingToken);
        Task processTask = _processor.ProcessAsync(channel.Reader, stoppingToken);
        await Task.WhenAll(readTask, processTask);
    }
}