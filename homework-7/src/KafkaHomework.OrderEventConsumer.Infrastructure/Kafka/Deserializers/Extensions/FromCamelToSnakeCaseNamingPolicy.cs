using System;
using System.Linq;
using System.Text.Json;

namespace KafkaHomework.OrderEventConsumer.Infrastructure.Kafka.Deserializers.Extensions;

public class FromCamelToSnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        var snakeCase = string.Concat(
            name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString().ToLower() : x.ToString().ToLower())
        );

        return snakeCase;
    }
}

