using Confluent.Kafka;
using Dapper;
using KafkaHomework.OrderEventConsumer.Domain.Sales;
using KafkaHomework.OrderEventConsumer.Domain.ValueObjects;
using KafkaHomework.OrderEventConsumer.Infrastructure.Models;
using KafkaHomework.OrderEventConsumer.Infrastructure.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaHomework.OrderEventConsumer.Infrastructure.Repositories;

public sealed class PositionRepository : PgRepository, IPositionRepository
{
    public PositionRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<long[]> Add(PositionEntity[] positions, CancellationToken token)
    {
        const string sqlQuery = @"
insert into positions (order_id, item_id, quantity, price_currency, price_amount)
values (@OrderId, @ItemId, @Quantity, @PriceCurrency, @PriceAmount)
returning id;
    ";

        await using var connection = await GetConnection();

        var ids = new List<long>();

        foreach (var position in positions)
        {
            var id = await connection.QueryAsync<long>(
                new CommandDefinition(
                    sqlQuery,
                    new
                    {
                        OrderId = position.OrderId,
                        ItemId = position.ItemId,
                        Quantity = position.Quantity,
                        PriceCurrency = position.PriceCurrency,
                        PriceAmount = position.PriceAmount,
                    },
                    cancellationToken: token));

            ids.Add(id.Single());
        }

        return ids.ToArray();
    }
}
