using FluentMigrator;

namespace Route256.Week5.Workshop.PriceCalculator.Dal.Migrations;

[Migration(20240828172500, TransactionBehavior.None)]
public class AddColumnsToTaskComment : Migration {
    public override void Up()
    {
        Alter.Table("task_comments")
            .AddColumn("modified_at").AsDateTimeOffset().Nullable()
            .AddColumn("deleted_at").AsDateTimeOffset().Nullable();
    }

    public override void Down()
    {
        Delete.Column("modified_at").FromTable("task_comments");
        Delete.Column("deleted_at").FromTable("task_comments");
    }
}