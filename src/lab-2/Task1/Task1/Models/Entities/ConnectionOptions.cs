namespace Task1.Models.Entities;

public class ConnectionOptions
{
    public string ConnectionHost { get; init; } = string.Empty;

    public int ConnectionPort { get; init; }

    public ConnectionOptions() { }

    public ConnectionOptions(string connectionHost, int connectionPort)
    {
        ConnectionHost = connectionHost;
        ConnectionPort = connectionPort;
    }
}