using System;

namespace KafkaHomework.OrderEventConsumer.Infrastructure.Models;

public record ItemInventoryEntity
{
    public long Id { get; init; }

    public required long ItemId { get; init; }

    public required int Reserved { get; init; }

    public required int Sold { get; init; }

    public required int Cancelled { get; init; }

    public required DateTimeOffset At { get; init; }
}
