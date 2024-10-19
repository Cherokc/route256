using Confluent.Kafka;
using KafkaHomework.OrderEventConsumer.Infrastructure.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KafkaHomework.OrderEventConsumer.Presentation.Kafka;

public static class KafkaServiceExtensions
{
    public static IServiceCollection AddKafkaHandler<TKey, TValue, THandler>(
        this IServiceCollection services,
        IDeserializer<TKey>? keySerializer,
        IDeserializer<TValue>? valueSerializer)
        where THandler : class, IHandler<TKey, TValue>
    {
        services.AddSingleton<THandler>();

        services.AddSingleton(sp =>
        {
            var handler = sp.GetRequiredService<THandler>();
            var logger = sp.GetRequiredService<ILogger<KafkaAsyncConsumer<TKey, TValue>>>();
            var kafkaOptions = sp.GetRequiredService<IOptions<KafkaOptions>>();

            return new KafkaAsyncConsumer<TKey, TValue>(handler, kafkaOptions, keySerializer, valueSerializer, logger);
        });

        return services;
    }
}

