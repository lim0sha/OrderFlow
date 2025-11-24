using System.ComponentModel.DataAnnotations;

namespace Task1.Models.Entities;

public class ConnectionOptions
{
    [Required]
    public string ConnectionHost { get; init; } = string.Empty;

    [Required]
    public int ConnectionPort { get; init; }

    public ConnectionOptions() { }

    public ConnectionOptions(string connectionHost, int connectionPort)
    {
        ConnectionHost = connectionHost;
        ConnectionPort = connectionPort;
    }
}