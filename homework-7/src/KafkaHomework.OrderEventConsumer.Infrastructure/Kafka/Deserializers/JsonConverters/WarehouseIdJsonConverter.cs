using KafkaHomework.OrderEventConsumer.Domain.ValueObjects;
using System;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace KafkaHomework.OrderEventConsumer.Infrastructure.Kafka.Deserializers.JsonConverters;

public class WarehouseIdJsonConverter : JsonConverter<WarehouseId>
{
    public override WarehouseId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new WarehouseId(reader.GetInt64());
    }

    public override void Write(Utf8JsonWriter writer, WarehouseId value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }
}
