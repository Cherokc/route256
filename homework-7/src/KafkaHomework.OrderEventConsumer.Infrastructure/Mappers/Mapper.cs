using KafkaHomework.OrderEventConsumer.Domain.Order;
using KafkaHomework.OrderEventConsumer.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KafkaHomework.OrderEventConsumer.Infrastructure.Mappers;

public static class Mapper
{
    public static OrderEventEntity ToOrderEventEntity(OrderEvent orderEvent)
    {
        return new OrderEventEntity()
        {
            OrderId = orderEvent.OrderId.Value,
            UserId = orderEvent.UserId.Value,
            WarehouseId = orderEvent.WarehouseId.Value,
            Moment = orderEvent.Moment,
            Status = (int)orderEvent.Status
        };
    }

    public static PositionEntity[] ToPositionEntities(OrderEvent orderEvent)
    {
        return orderEvent.Positions
            .Select(p => new PositionEntity()
            {
                OrderId = orderEvent.OrderId.Value,
                ItemId = p.ItemId.Value,
                Quantity = p.Quantity,
                PriceCurrency = p.Price.Currency,
                PriceAmount = p.Price.Value
            })
            .ToArray();
    }
}
