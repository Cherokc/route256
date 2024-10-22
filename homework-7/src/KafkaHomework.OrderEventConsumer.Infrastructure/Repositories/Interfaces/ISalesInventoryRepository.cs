using KafkaHomework.OrderEventConsumer.Domain.Sales;
using KafkaHomework.OrderEventConsumer.Infrastructure.Models;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaHomework.OrderEventConsumer.Infrastructure.Repositories.Interfaces;

public interface ISalesInventoryRepository
{
    Task<long> Add(SalesInventoryEntity salesInventory, CancellationToken token);
    Task<SalesInventoryEntity?> GetLast(long sellerId, long itemId, string priceCurrency, CancellationToken token);
    Task<SalesInventoryEntity[]> Get(long sellerId, long itemId, CancellationToken token);
}
