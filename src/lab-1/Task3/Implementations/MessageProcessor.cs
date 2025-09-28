using System.Threading.Channels;
using Task3.Entities;
using Task3.Interfaces;

namespace Task3.Implementations;

public class MessageProcessor : IMessageSender, IMessageProcessor
{
    private readonly Channel<Message> _channel;
    private readonly long _batchVolume;
    private readonly TimeSpan _batchTime;
    private readonly IMessageHandler _handler;

    public MessageProcessor(long channelsVolume, IMessageHandler handler, long batchVolume, TimeSpan? batchTime = null)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _batchVolume = batchVolume;
        _batchTime = batchTime ?? TimeSpan.FromMilliseconds(100);

        var boundedOptions = new BoundedChannelOptions((int)channelsVolume)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false,
        };

        _channel = Channel.CreateBounded<Message>(boundedOptions);
    }

    public void Complete()
    {
        _channel.Writer.Complete();
    }

    public async ValueTask SendAsync(Message message, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(message);
        await _channel.Writer.WriteAsync(message, cancellationToken);
    }

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        IAsyncEnumerable<Message> messageList = _channel.Reader.ReadAllAsync(cancellationToken);

        await foreach (IReadOnlyList<Message> chunk in messageList.ChunkAsync((int)_batchVolume, _batchTime).WithCancellation(cancellationToken))
        {
            await _handler.HandleAsync(chunk, cancellationToken).ConfigureAwait(false);
        }
    }
}