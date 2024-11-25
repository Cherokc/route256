using System;

namespace KafkaHomework.OrderEventConsumer.Infrastructure.Models;

public record SalesInventoryEntity
{
    public long Id { get; init; }

    public required long SellerId { get; init; }

    public required long ItemId { get; init; }

    public required int Quantity { get; init; }

    public required string PriceCurrency { get; init; }

    public required decimal PriceAmount { get; init; }

    public required DateTimeOffset At { get; init; }
}