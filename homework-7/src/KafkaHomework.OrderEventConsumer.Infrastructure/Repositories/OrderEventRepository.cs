using Dapper;
using KafkaHomework.OrderEventConsumer.Infrastructure.Repositories.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using KafkaHomework.OrderEventConsumer.Infrastructure.Models;

namespace KafkaHomework.OrderEventConsumer.Infrastructure.Repositories;

public sealed class OrderEventRepository : PgRepository, IOrderEventRepository
{
    public OrderEventRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task Add(OrderEventEntity[] orderEvents, CancellationToken token)
    {
        const string sqlQuery = @"
insert into order_events (order_id, user_id, warehouse_id, status, moment)
values (@OrderId, @UserId, @WarehouseId, @Status, @Moment)
";

        await using var connection = await GetConnection();
        
        foreach(var orderEvent in orderEvents)
            await connection.QueryAsync<long>(
                new CommandDefinition(
                    sqlQuery,
                    new
                    {
                        OrderId = orderEvent.OrderId,
                        UserId = orderEvent.UserId,
                        WarehouseId = orderEvent.WarehouseId,
                        Status = orderEvent.Status,
                        Moment = orderEvent.Moment,
                    },
                    cancellationToken: token));
    }
}
