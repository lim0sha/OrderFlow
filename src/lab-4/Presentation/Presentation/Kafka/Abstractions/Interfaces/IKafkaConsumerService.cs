using Microsoft.Extensions.Hosting;

namespace Presentation.Kafka.Abstractions.Interfaces;

public interface IKafkaConsumerService<TKey, TValue> : IHostedService { }