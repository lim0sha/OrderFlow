using Confluent.Kafka;
using Google.Protobuf;
using Microsoft.Extensions.Options;
using Presentation.Kafka.Abstractions.Interfaces;
using Presentation.Kafka.Abstractions.Options;

namespace Presentation.Kafka.Abstractions.Services;

public class KafkaProducer<TKey, TValue> : IKafkaProducer<TKey, TValue>
    where TValue : IMessage<TValue>, new()
{
    private readonly IProducer<TKey, TValue> _producer;
    private readonly string _topic;

    public KafkaProducer(
        IOptions<KafkaOptions> options,
        string topic,
        ISerializer<TKey> keySerializer,
        ISerializer<TValue> valueSerializer)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            MessageSendMaxRetries = 3,
            ApiVersionRequestTimeoutMs = 500,
        };

        ProducerBuilder<TKey, TValue> builder = new ProducerBuilder<TKey, TValue>(config)
            .SetKeySerializer(keySerializer)
            .SetValueSerializer(valueSerializer);

        _producer = builder.Build();
        _topic = topic;
    }

    public async Task ProduceAsync(TKey key, TValue value, CancellationToken ct = default)
    {
        await _producer.ProduceAsync(_topic, new Message<TKey, TValue> { Key = key, Value = value }, ct);
    }

    public void Dispose()
    {
        _producer.Dispose();
    }
}