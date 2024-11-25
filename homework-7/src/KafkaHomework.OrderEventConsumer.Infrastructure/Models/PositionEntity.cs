using System;

namespace KafkaHomework.OrderEventConsumer.Infrastructure.Models;

public record PositionEntity
{
    public long Id { get; init; }

    public required long OrderId { get; init; }

    public required long ItemId { get; init; }

    public required long Quantity { get; init; }

    public required string PriceCurrency { get; init; }

    public required decimal PriceAmount { get; init; }
}
