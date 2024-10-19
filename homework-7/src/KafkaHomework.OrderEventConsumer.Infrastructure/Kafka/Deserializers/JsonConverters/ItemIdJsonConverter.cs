using KafkaHomework.OrderEventConsumer.Domain.ValueObjects;
using System;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace KafkaHomework.OrderEventConsumer.Infrastructure.Kafka.Deserializers.JsonConverters;

public class ItemIdJsonConverter : JsonConverter<ItemId>
{
    public override ItemId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new ItemId(reader.GetInt64());
    }

    public override void Write(Utf8JsonWriter writer, ItemId value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }
}
