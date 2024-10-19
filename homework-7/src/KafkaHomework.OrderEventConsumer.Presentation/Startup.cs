using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using KafkaHomework.OrderEventConsumer.Infrastructure.Common;
using KafkaHomework.OrderEventConsumer.Infrastructure.Kafka;
using KafkaHomework.OrderEventConsumer.Domain.Order;
using KafkaHomework.OrderEventConsumer.Infrastructure.Kafka.Deserializers;
using KafkaHomework.OrderEventConsumer.Presentation.Kafka;
using KafkaHomework.OrderEventConsumer.Infrastructure.Repositories.Interfaces;
using KafkaHomework.OrderEventConsumer.Infrastructure.Repositories;
using Ozon.Route256.Postgres.Persistence.Migrations;
using FluentMigrator.Runner;
using KafkaHomework.OrderEventConsumer.Infrastructure;
using KafkaHomework.OrderEventConsumer.Infrastructure.Models;
using Npgsql;

namespace KafkaHomework.OrderEventConsumer.Presentation;

public sealed class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration) => _configuration = configuration;

    [System.Obsolete]
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddLogging();

        var connectionString = _configuration["ConnectionString"]!;

        Postgres.AddMigrations(services, connectionString);

        services.AddSingleton<IOrderEventRepository, OrderEventRepository>(_ => new OrderEventRepository(connectionString));
        services.AddSingleton<IPositionRepository, PositionRepository>(_ => new PositionRepository(connectionString));
        services.AddSingleton<IItemInventoryRepository, ItemInventoryRepository>(_ => new ItemInventoryRepository(connectionString));

        services.Configure<KafkaOptions>(_configuration.GetSection("KafkaConsumerOptions"));

        services.AddKafkaHandler<long, OrderEvent, ItemHandler>(null, new OrderEventDeserializer());

        services.AddHostedService<KafkaBackgroundService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }
    }
}
