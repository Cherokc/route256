using Npgsql;
using System.Threading.Tasks;
using System.Transactions;

namespace KafkaHomework.OrderEventConsumer.Infrastructure.Repositories.Interfaces;
public abstract class PgRepository : IPgRepository
{
    private readonly string _connectionString;

    protected const int DefaultTimeoutInSeconds = 5;

    public PgRepository(string connectionString) => _connectionString = connectionString;

    public async Task<NpgsqlConnection> GetConnection()
    {
        if (Transaction.Current is not null &&
            Transaction.Current.TransactionInformation.Status is TransactionStatus.Aborted)
        {
            throw new TransactionAbortedException("Transaction was aborted (probably by user cancellation request)");
        }

        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        // Due to in-process migrations
        connection.ReloadTypes();

        return connection;
    }
}