using Dapper;
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

    public async Task Update(ItemInventoryEntity itemInventory, CancellationToken token)
    {
        const string sqlQuery = @"
insert into item_inventories (item_id, reserved, sold, cancelled, at)
values (@ItemId, @Reserved, @Sold, @Cancelled, @At)
    on conflict (item_id)
    do update set
       reserved = item_inventories.reserved + excluded.reserved
     , sold = item_inventories.sold + excluded.sold
     , cancelled = item_inventories.cancelled + excluded.cancelled
     , at = excluded.at;
";

        await using var connection = await GetConnection();
        await connection.QueryAsync<long>(
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
    }

    public async Task<ItemInventoryEntity?> Get(long itemId, CancellationToken token)
    {
        const string sqlQuery = @"
select *
  from item_inventories as ii
 where ii.item_id = @ItemId
";

        await using var connection = await GetConnection();
        return await connection.QuerySingleOrDefaultAsync<ItemInventoryEntity>(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    ItemId = itemId
                },
                cancellationToken: token));
    }
}