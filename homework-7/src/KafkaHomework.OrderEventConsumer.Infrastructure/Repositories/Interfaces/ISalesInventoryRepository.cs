using KafkaHomework.OrderEventConsumer.Infrastructure.Models;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaHomework.OrderEventConsumer.Infrastructure.Repositories.Interfaces;

public interface ISalesInventoryRepository
{
    Task<long> Update(SalesInventoryEntity salesInventory, CancellationToken token);
    Task<SalesInventoryEntity[]> Get(long sellerId, long itemId, CancellationToken token);
}
