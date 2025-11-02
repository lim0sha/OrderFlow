namespace Gateway.Configs;

public class GrpcConnectionOptions
{
    public string OrderServiceAddress { get; init; } = string.Empty;

    public string ProductServiceAddress { get; init; } = string.Empty;
}