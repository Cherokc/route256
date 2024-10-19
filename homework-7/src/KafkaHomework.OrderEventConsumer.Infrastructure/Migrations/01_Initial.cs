using System;
using FluentMigrator;

using KafkaHomework.OrderEventConsumer.Infrastructure.Common;

namespace Ozon.Route256.Postgres.Persistence.Migrations;

[Migration(1, "Initial migration")]
public class InitSchema : Migration
{
    public override void Up()
    {
        Create.Table("order_events")
            .WithColumn("order_id").AsInt64().NotNullable()
            .WithColumn("user_id").AsInt64().NotNullable()
            .WithColumn("warehouse_id").AsInt64().NotNullable()
            .WithColumn("status").AsInt32().NotNullable()
            .WithColumn("moment").AsDateTimeOffset().NotNullable();

        Create.Table("positions")
            .WithColumn("id").AsInt64().Identity()
            .WithColumn("order_id").AsInt64().NotNullable()
            .WithColumn("item_id").AsInt64().NotNullable()
            .WithColumn("quantity").AsInt32().NotNullable()
            .WithColumn("price_currency").AsString().NotNullable()
            .WithColumn("price_amount").AsDecimal().NotNullable();

        Create.Table("item_inventories")
            .WithColumn("id").AsInt64().Identity()
            .WithColumn("item_id").AsInt64().NotNullable()
            .WithColumn("reserved").AsInt32().NotNullable()
            .WithColumn("sold").AsInt32().NotNullable()
            .WithColumn("cancelled").AsInt32().NotNullable()
            .WithColumn("at").AsDateTimeOffset().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("order_events");
        Delete.Table("positions");
        Delete.Table("item_inventories");
    }
}