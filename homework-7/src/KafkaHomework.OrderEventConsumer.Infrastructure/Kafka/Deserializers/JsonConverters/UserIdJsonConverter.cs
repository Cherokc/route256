using KafkaHomework.OrderEventConsumer.Domain.ValueObjects;
using System;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace KafkaHomework.OrderEventConsumer.Infrastructure.Kafka.Deserializers.JsonConverters;

public class UserIdJsonConverter : JsonConverter<UserId>
{
    public override UserId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new UserId(reader.GetInt64());
    }

    public override void Write(Utf8JsonWriter writer, UserId value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }
}
