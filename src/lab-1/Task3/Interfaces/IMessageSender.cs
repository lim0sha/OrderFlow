using Task3.Entities;

namespace Task3.Interfaces;

public interface IMessageSender
{
    ValueTask SendAsync(Message message, CancellationToken cancellationToken);
}