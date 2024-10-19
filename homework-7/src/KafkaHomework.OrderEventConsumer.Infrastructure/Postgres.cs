using FluentMigrator.Runner;
using KafkaHomework.OrderEventConsumer.Infrastructure.Common;
using KafkaHomework.OrderEventConsumer.Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using Npgsql.NameTranslation;
using Ozon.Route256.Postgres.Persistence.Migrations;

namespace KafkaHomework.OrderEventConsumer.Infrastructure;

public static class Postgres
{
    private static readonly INpgsqlNameTranslator Translator = new NpgsqlSnakeCaseNameTranslator();

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
