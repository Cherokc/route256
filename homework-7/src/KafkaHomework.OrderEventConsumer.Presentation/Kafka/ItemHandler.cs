using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using KafkaHomework.OrderEventConsumer.Infrastructure.Kafka;
using KafkaHomework.OrderEventConsumer.Domain.Order;
using KafkaHomework.OrderEventConsumer.Infrastructure.Repositories.Interfaces;
using System.Linq;
using KafkaHomework.OrderEventConsumer.Infrastructure.Models;
using KafkaHomework.OrderEventConsumer.Infrastructure.Mappers;
using KafkaHomework.OrderEventConsumer.Domain.ValueObjects;

namespace KafkaHomework.OrderEventConsumer.Presentation.Kafka;

public class ItemHandler : IHandler<long, OrderEvent>
{
    private readonly ILogger<ItemHandler> _logger;
    private readonly IOrderEventRepository _orderEventRepository;
    private readonly IPositionRepository _positionRepository;
    private readonly IItemInventoryRepository _itemInventoryRepository;
    private readonly ISalesInventoryRepository _salesInventoryRepository;
    private readonly Random _random = new();

    public ItemHandler(
        ILogger<ItemHandler> logger,
        IOrderEventRepository orderEventRepository,
        IPositionRepository positionRepository,
        IItemInventoryRepository itemInventoryRepository,
        ISalesInventoryRepository salesInventoryRepository)
    {
        _logger = logger;
        _orderEventRepository = orderEventRepository;
        _positionRepository = positionRepository;
        _itemInventoryRepository = itemInventoryRepository;
        _salesInventoryRepository = salesInventoryRepository;
    }

    public async Task Handle(IReadOnlyCollection<ConsumeResult<long, OrderEvent>> messages, CancellationToken token)
    {
        await Task.Delay(_random.Next(300), token);
        _logger.LogInformation("Handled {Count} messages", messages.Count);

        var orderEvents = messages
            .Select(m => m.Message.Value)
            .ToArray();

        await InsertOrderEvents(orderEvents, token);
        await InsertPositions(orderEvents, token);
        await UpdateItemInventories(orderEvents, token);
        await UpdateSalesInventories(orderEvents, token);
    }

    private async Task InsertOrderEvents(OrderEvent[] orderEvents, CancellationToken token)
    {
        var orderEventModels = orderEvents
            .Select(Mapper.ToOrderEventEntity)
            .ToArray();

        await _orderEventRepository.Add(orderEventModels, token);
    }

    private async Task InsertPositions(OrderEvent[] orderEvents, CancellationToken token)
    {
        var positionModels = orderEvents
            .SelectMany(Mapper.ToPositionEntities)
            .ToArray();

        await _positionRepository.Add(positionModels, token);
    }

    private async Task UpdateItemInventories(OrderEvent[] orderEvents, CancellationToken token)
    {
        foreach (var orderEvent in orderEvents)
            foreach (var position in orderEvent.Positions)
            {
                var itemInventory = new ItemInventoryEntity()
                {
                    ItemId = position.ItemId.Value,
                    Reserved = orderEvent.Status == Status.Created ? position.Quantity : -position.Quantity,
                    Sold = orderEvent.Status == Status.Delivered ? position.Quantity : 0,
                    Cancelled = orderEvent.Status == Status.Cancelled ? position.Quantity : 0,
                    At = orderEvent.Moment
                };

                await _itemInventoryRepository.Update(itemInventory, token);
            }
    }

    private async Task UpdateSalesInventories(OrderEvent[] orderEvents, CancellationToken token)
    {
        foreach (var orderEvent in orderEvents)
            foreach (var position in orderEvent.Positions)
            {
                var sellerId = position.ItemId.Value / 1_000_000;
                var itemId = position.ItemId.Value % 1_000_000;

                var salesInventory = new SalesInventoryEntity()
                    {
                        SellerId = sellerId,
                        ItemId = itemId,
                        PriceCurrency = position.Price.Currency,
                        PriceAmount = position.Price.Value,
                        Quantity = position.Quantity,
                        At = orderEvent.Moment
                    };

                await _salesInventoryRepository.Update(salesInventory, token);
            }
    }
}
