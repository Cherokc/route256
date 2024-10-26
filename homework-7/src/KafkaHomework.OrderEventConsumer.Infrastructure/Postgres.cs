using FluentMigrator.Runner;
using KafkaHomework.OrderEventConsumer.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaHomework.OrderEventConsumer.Infrastructure;

public static class Postgres
{
    /// <summary>
    /// Add migration infrastructure
    /// </summary>
    public static void AddMigrations(IServiceCollection services, string connectionString)
    {
        services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb.AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(SqlMigration).Assembly).For.Migrations()
            )
            .AddLogging(lb => lb.AddFluentMigratorConsole());
    }
}
