using Npgsql;
using System.Threading.Tasks;

namespace KafkaHomework.OrderEventConsumer.Infrastructure.Repositories.Interfaces;
public interface IPgRepository
{
    Task<NpgsqlConnection> GetConnection();
}