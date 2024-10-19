using KafkaHomework.OrderEventConsumer.Domain.ValueObjects;
using System;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace KafkaHomework.OrderEventConsumer.Infrastructure.Kafka.Deserializers.JsonConverters;

public class MoneyJsonConverter : JsonConverter<Money>
{
    public override Money Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var root = doc.RootElement;
            decimal units = root.GetProperty("units").GetInt64();
            int nanos = root.GetProperty("nanos").GetInt32();
            string currency = root.GetProperty("currency").GetString() ?? "";

            decimal value = units + nanos / 1_000_000_000M;
            return new Money(value, currency);
        }
    }

    public override void Write(Utf8JsonWriter writer, Money value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("units", (long)value.Value);
        writer.WriteNumber("nanos", (int)((value.Value - (long)value.Value) * 1_000_000_000));
        writer.WriteString("currency", value.Currency);
        writer.WriteEndObject();
    }
}
