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

public sealed class SalesInventoryRepository : PgRepository, ISalesInventoryRepository
{
    public SalesInventoryRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long> Add(SalesInventoryEntity salesInventory, CancellationToken token)
    {
        const string sqlQuery = @"
insert into sales_inventories (seller_id, item_id, quantity, price_currency, price_amount, at)
values (@SellerId, @ItemId, @Quantity, @PriceCurrency, @PriceAmount, @At)
returning id;
";

        await using var connection = await GetConnection();
        var ids = await connection.QueryAsync<long>(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    SellerId = salesInventory.SellerId,
                    ItemId = salesInventory.ItemId,
                    Quantity = salesInventory.Quantity,
                    PriceCurrency = salesInventory.PriceCurrency,
                    PriceAmount = salesInventory.PriceAmount,
                    At = salesInventory.At,
                },
                cancellationToken: token));

        return ids.Single();
    }


    public async Task<SalesInventoryEntity[]> Get(long sellerId, long itemId, CancellationToken token)
    {
        const string sqlQuery = @"
select si.id as Id
     , si.seller_id as SellerId
     , si.item_id as ItemId
     , si.quantity as Quantity
     , si.price_currency as PriceCurrency
     , si.price_amount as PriceAmount
     , si.at as At
  from sales_inventories as si
 where si.seller_id = @SellerId
   and si.item_id = @ItemId
 order by si.at desc
";

        await using var connection = await GetConnection();
        var items = await connection.QueryAsync<SalesInventoryEntity>(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    SellerId = sellerId,
                    ItemId = itemId,
                },
                cancellationToken: token));

        return items
            .ToArray();
    }

    public async Task<SalesInventoryEntity?> GetLast(long sellerId, long itemId, string priceCurrency, CancellationToken token)
    {
        const string sqlQuery = @"
select si.id as Id
     , si.seller_id as SellerId
     , si.item_id as ItemId
     , si.quantity as Quantity
     , si.price_currency as PriceCurrency
     , si.price_amount as PriceAmount
     , si.at as At
  from sales_inventories as si
 where si.seller_id = @SellerId
   and si.item_id = @ItemId
   and si.price_currency = @PriceCurrency
 order by si.at desc
 limit 1;
";

        await using var connection = await GetConnection();
        var items = await connection.QueryAsync<SalesInventoryEntity>(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    SellerId = sellerId,
                    ItemId = itemId,
                    PriceCurrency = priceCurrency,
                },
                cancellationToken: token));

        return items
            .FirstOrDefault();
    }
}