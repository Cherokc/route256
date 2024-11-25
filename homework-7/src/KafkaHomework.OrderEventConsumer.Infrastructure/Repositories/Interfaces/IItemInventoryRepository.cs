using KafkaHomework.OrderEventConsumer.Infrastructure.Models;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaHomework.OrderEventConsumer.Infrastructure.Repositories.Interfaces;

public interface IItemInventoryRepository
{
    Task Update(ItemInventoryEntity itemInventory, CancellationToken token);
    Task<ItemInventoryEntity?> Get(long itemId, CancellationToken token);
}
