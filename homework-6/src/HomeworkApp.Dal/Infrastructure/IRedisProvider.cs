using System.Threading.Tasks;
using StackExchange.Redis;

namespace HomeworkApp.Dal.Infrastructure;

public interface IRedisProvider
{
    Task<IDatabase> GetConnection();
}