using System.Threading.Channels;
using Task3.Configs;
using Task3.Entities;
using Task3.Interfaces;

namespace Task3.Implementations;

public class MessageProcessor : IMessageSender, IMessageProcessor
{
    private readonly Channel<Message> _channel;
    private readonly int _batchVolume;
    private readonly TimeSpan _batchTime;
    private readonly List<IMessageHandler> _handlers;

    public MessageProcessor(
        MessageProcessorConfig config,
        IEnumerable<IMessageHandler> handlers)
    {
        var channelOptions = new BoundedChannelOptions(config.ChannelCapacity)
        {
            FullMode = config.FullMode,
            SingleReader = config.SingleReader,
            SingleWriter = config.SingleWriter,
        };

        _channel = Channel.CreateBounded<Message>(channelOptions);
        _batchVolume = config.BatchVolume;
        _batchTime = config.BatchTime;
        _handlers = handlers.ToList();

        if (_handlers.Count == 0)
            throw new ArgumentException("Error in handlers", nameof(handlers));
    }

    public void Complete()
    {
        _channel.Writer.Complete();
    }

    public async ValueTask SendAsync(Message message, CancellationToken cancellationToken)
    {
        await _channel.Writer.WriteAsync(message, cancellationToken).ConfigureAwait(false);
    }

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        IAsyncEnumerable<Message> messageStream = _channel.Reader.ReadAllAsync(cancellationToken);

        await foreach (IReadOnlyList<Message>? chunk in messageStream
                           .ChunkAsync(_batchVolume, _batchTime)
                           .WithCancellation(cancellationToken))
        {
            var handlerTasks = _handlers
                .Select(h => h.HandleAsync(chunk, cancellationToken).AsTask())
                .ToList();

            await Task.WhenAll(handlerTasks);
        }
    }
}