using Task3.Entities;
using Task3.Interfaces;

namespace Task3.Implementations;

public class MessageHandler : IMessageHandler
{
    public ValueTask HandleAsync(IEnumerable<Message> messages, CancellationToken ct)
    {
        var sb = new System.Text.StringBuilder();
        foreach (Message msg in messages)
        {
            sb.AppendLine($"{msg.Title}: {msg.Text}");
        }

        System.Console.WriteLine(sb.ToString());
        return ValueTask.CompletedTask;
    }
}