using KafkaHomework.OrderEventConsumer.Domain.Sales;
using KafkaHomework.OrderEventConsumer.Infrastructure.Models;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaHomework.OrderEventConsumer.Infrastructure.Repositories.Interfaces;

public interface IItemInventoryRepository
{
    Task<long> Add(ItemInventoryEntity itemInventory, CancellationToken token);
    Task<ItemInventoryEntity?> GetLast(long itemId, CancellationToken token);
    Task<ItemInventoryEntity[]> Get(long itemId, CancellationToken token);
}
