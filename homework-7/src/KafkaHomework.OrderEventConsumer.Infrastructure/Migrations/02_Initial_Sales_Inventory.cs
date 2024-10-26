using FluentMigrator;

namespace Ozon.Route256.Postgres.Persistence.Migrations;

[Migration(2, "Initial sales inventory migration")]
public class SalesInitSchema : Migration
{
    public override void Up()
    {
        Create.Table("sales_inventories")
            .WithColumn("id").AsInt64().Identity()
            .WithColumn("seller_id").AsInt64().NotNullable()
            .WithColumn("item_id").AsInt64().NotNullable()
            .WithColumn("quantity").AsInt32().NotNullable()
            .WithColumn("price_currency").AsString().NotNullable()
            .WithColumn("price_amount").AsDecimal().NotNullable()
            .WithColumn("at").AsDateTimeOffset().NotNullable(); 
        
        Create.UniqueConstraint("uq_sales_inventories")
            .OnTable("sales_inventories")
            .Columns("seller_id", "item_id", "price_currency");
    }

    public override void Down()
    {
        Delete.Table("sales_inventories");
    }
}