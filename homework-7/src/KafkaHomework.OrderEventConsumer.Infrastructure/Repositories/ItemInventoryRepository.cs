using Confluent.Kafka;
using Dapper;
using KafkaHomework.OrderEventConsumer.Domain.Sales;
using KafkaHomework.OrderEventConsumer.Domain.ValueObjects;
using KafkaHomework.OrderEventConsumer.Infrastructure.Models;
using KafkaHomework.OrderEventConsumer.Infrastructure.Repositories.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaHomework.OrderEventConsumer.Infrastructure.Repositories;

public sealed class ItemInventoryRepository : PgRepository, IItemInventoryRepository
{
    public ItemInventoryRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> Add(ItemInventoryEntity itemInventory, CancellationToken token)
    {
        const string sqlQuery = @"
insert into item_inventories (item_id, reserved, sold, cancelled, at)
values (@ItemId, @Reserved, @Sold, @Cancelled, @At)
returning id;
";

        if (itemInventory.ItemId == 0)
            Console.WriteLine();

        await using var connection = await GetConnection();
        var ids = await connection.QueryAsync<long>(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    ItemId = itemInventory.ItemId,
                    Reserved = itemInventory.Reserved,
                    Sold = itemInventory.Sold,
                    Cancelled = itemInventory.Cancelled,
                    At = itemInventory.At,
                },
                cancellationToken: token));

        return ids.Single();
    }

    public async Task<ItemInventoryEntity[]> Get(long itemId, CancellationToken token)
    {
        const string sqlQuery = @"
select *
  from item_inventories as ii
 where ii.item_id = @ItemId
 order by ii.at desc
";

        await using var connection = await GetConnection();
        var items = await connection.QueryAsync<ItemInventoryEntity>(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    ItemId = itemId
                },
                cancellationToken: token));

        return items
            .ToArray();
    }

    public async Task<ItemInventoryEntity?> GetLast(long itemId, CancellationToken token)
    {
        const string sqlQuery = @"
select *
  from item_inventories as ii
 where ii.item_id = @ItemId
 order by ii.at desc
 limit 1;
";

        await using var connection = await GetConnection();
        var items = await connection.QueryAsync<ItemInventoryEntity>(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    ItemId = itemId
                },
                cancellationToken: token));

        return items
            .FirstOrDefault();
    }
}
