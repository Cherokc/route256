using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using KafkaHomework.OrderEventConsumer.Infrastructure.Kafka;
using Microsoft.Extensions.Options;
using KafkaHomework.OrderEventConsumer.Domain.Order;

namespace KafkaHomework.OrderEventConsumer.Presentation.Kafka;

public class KafkaBackgroundService : BackgroundService
{
    private readonly KafkaAsyncConsumer<long, OrderEvent> _consumer;
    private readonly ILogger<KafkaBackgroundService> _logger;

    public KafkaBackgroundService(IServiceProvider serviceProvider,
                                  ILogger<KafkaBackgroundService> logger)
    {
        _logger = logger;
        _consumer = serviceProvider.GetRequiredService<KafkaAsyncConsumer<long, OrderEvent>>();
    }


    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _consumer.Dispose();

        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _consumer.Consume(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occured");
        }
    }
}
