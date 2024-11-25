using KafkaHomework.OrderEventConsumer.Infrastructure.Models;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaHomework.OrderEventConsumer.Infrastructure.Repositories.Interfaces;

public interface IOrderEventRepository
{
    Task Add(OrderEventEntity[] orderEvents, CancellationToken token);
}
