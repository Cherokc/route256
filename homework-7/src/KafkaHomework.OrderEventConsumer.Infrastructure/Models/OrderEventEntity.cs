using System;

namespace KafkaHomework.OrderEventConsumer.Infrastructure.Models;

public record OrderEventEntity
{
    public required long OrderId { get; init; }

    public required long UserId { get; init; }

    public required long WarehouseId { get; init; }

    public required DateTimeOffset Moment { get; init; }

    public required int Status { get; init; }
}
