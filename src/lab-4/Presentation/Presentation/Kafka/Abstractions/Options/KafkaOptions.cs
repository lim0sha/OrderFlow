namespace Presentation.Kafka.Abstractions.Options;

public class KafkaOptions
{
    public string BootstrapServers { get; set; } = "localhost:9092";

    public string OrderCreationTopic { get; set; } = "order_creation";

    public string OrderProcessingTopic { get; set; } = "order_processing";

    public int BatchSize { get; set; } = 100;

    public int PollTimeoutMs { get; set; } = 1000;
}