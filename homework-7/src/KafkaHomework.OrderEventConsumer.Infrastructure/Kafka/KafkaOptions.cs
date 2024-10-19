using System;
using System.Collections.Generic;
namespace KafkaHomework.OrderEventConsumer.Infrastructure.Kafka;

public class KafkaOptions
{
    public int ChannelCapacity { get; set; } = 10; 
    public int BufferDelayInSeconds { get; set; } = 1; 
    public string BootstrapServers { get; set; } = "kafka:9092"; 
    public string GroupId { get; set; } = "group_id";
    public string Topic { get; set; } = "order_events"; 
}
