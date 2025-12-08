using Confluent.Kafka;
using Google.Protobuf;
using Kafka.Abstractions.Interfaces;
using Kafka.Abstractions.Options;
using Kafka.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Kafka.Abstractions.Extensions;

public static class KafkaServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaProducer<TKey, TValue>(
        this IServiceCollection services,
        string topic,
        ISerializer<TKey> keySerializer,
        ISerializer<TValue> valueSerializer)
        where TValue : IMessage<TValue>, new()
    {
        services.AddSingleton<IKafkaProducer<TKey, TValue>>(sp =>
        {
            IOptions<KafkaOptions> options = sp.GetRequiredService<IOptions<KafkaOptions>>();
            return new KafkaProducer<TKey, TValue>(options, topic, keySerializer, valueSerializer);
        });
        return services;
    }
}