using Task3.Entities;

namespace Task3.Interfaces;

public interface IMessageHandler
{
    ValueTask HandleAsync(IEnumerable<Message> messages, CancellationToken ct);
}