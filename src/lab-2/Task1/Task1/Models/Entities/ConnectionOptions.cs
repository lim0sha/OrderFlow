namespace Task1.Models.Entities;

public class ConnectionOptions
{
    public string ConnectionHost { get; init; } = string.Empty;

    public int ConnectionPort { get; init; }

    public ConnectionOptions() { }

    public ConnectionOptions(string connectionHost, int connectionPort)
    {
        Validate(connectionHost, connectionPort);
        ConnectionHost = connectionHost;
        ConnectionPort = connectionPort;
    }

    private static void Validate(string host, int port)
    {
        if (string.IsNullOrWhiteSpace(host))
            throw new InvalidOperationException("ConnectionHost is required.");
        if (port is <= 0 or > 65535)
            throw new InvalidOperationException("ConnectionPort must be between 1 and 65535.");
    }
}