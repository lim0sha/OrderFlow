using Task3.Configs;
using Task3.Entities;
using Task3.Implementations;

namespace Task3.Console;

public static class Program
{
    public static async Task Main(string[] args)
    {
        const int channelsVolume = 3;
        const int batchVolume = 5;
        var batchTimeout = TimeSpan.FromMilliseconds(100);
        const int totalMessages = 1000;

        var messageHandler = new MessageHandler();
        var config = new MessageProcessorConfig
        {
            ChannelCapacity = channelsVolume,
            BatchVolume = batchVolume,
            BatchTime = batchTimeout,
        };

        var messageProcessor = new MessageProcessor(config, new[] { messageHandler });

        MessageProcessor processor = messageProcessor;
        MessageProcessor sender = messageProcessor;

        ArgumentNullException.ThrowIfNull(processor);
        ArgumentNullException.ThrowIfNull(sender);

        var cts = new CancellationTokenSource();
        Task processingTask = processor.ProcessAsync(cts.Token);

        IEnumerable<Message> messages = Enumerable.Range(1, totalMessages)
            .Select(n => new Message($"#{n}", "text"));

        await Parallel.ForEachAsync(
            messages,
            cts.Token,
            async (message, token) => await sender.SendAsync(message, token));

        processor.Complete();
        cts.Dispose();
        await processingTask;
    }
}