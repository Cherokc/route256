using Confluent.Kafka;
using System.Text.Json.Serialization;
using System.Text.Json;
using System;
using KafkaHomework.OrderEventConsumer.Domain.Order;
using KafkaHomework.OrderEventConsumer.Domain.ValueObjects;
using KafkaHomework.OrderEventConsumer.Infrastructure.Kafka.Deserializers.JsonConverters;
using KafkaHomework.OrderEventConsumer.Infrastructure.Kafka.Deserializers.Extensions;

namespace KafkaHomework.OrderEventConsumer.Infrastructure.Kafka.Deserializers;

public class OrderEventDeserializer : IDeserializer<OrderEvent>
{
    public OrderEvent Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        var json = System.Text.Encoding.UTF8.GetString(data);

        if (string.IsNullOrEmpty(json))
            throw new ArgumentNullException(nameof(json));

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = new FromCamelToSnakeCaseNamingPolicy(),
            Converters =
            {
                new OrderIdJsonConverter(),
                new UserIdJsonConverter(),
                new WarehouseIdJsonConverter(),
                new JsonStringEnumConverter(),
                new ItemIdJsonConverter(),
                new MoneyJsonConverter(),
            }
        };

        var orderEvent = JsonSerializer.Deserialize<OrderEvent>(json, options);

        if (orderEvent == null)
            throw new JsonException();

        return orderEvent;
    }
}
