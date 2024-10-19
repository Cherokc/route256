using KafkaHomework.OrderEventConsumer.Domain.ValueObjects;
using System;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace KafkaHomework.OrderEventConsumer.Infrastructure.Kafka.Deserializers;

public class OrderIdJsonConverter : JsonConverter<OrderId>
{
    public override OrderId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new OrderId(reader.GetInt64());
    }

    public override void Write(Utf8JsonWriter writer, OrderId value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }
}
