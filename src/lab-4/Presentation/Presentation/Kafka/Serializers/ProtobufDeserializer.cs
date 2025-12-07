using Confluent.Kafka;
using Google.Protobuf;

namespace Presentation.Kafka.Serializers;

public class ProtobufDeserializer<T> : IDeserializer<T>
    where T : IMessage<T>, new()
{
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull || data.Length == 0)
            return new T();

        var message = new T();
        message.MergeFrom(data);
        return message;
    }
}