using KafkaHomework.OrderEventConsumer.Domain.ValueObjects;
using System;
using System.Reflection.Metadata;

namespace KafkaHomework.OrderEventConsumer.Domain.Sales;

public sealed record ItemInventory(
    ItemId ItemId,
    //Reserved Reserved,
    //Cancelled Cancelled,
    DateTime LastUpdate);
