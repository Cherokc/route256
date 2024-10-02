using System.Threading.Tasks;
using HomeworkApp.Dal.Settings;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace HomeworkApp.Dal.Infrastructure;

public class RedisProvider : IRedisProvider
{
    // we must reuse it as it possible
    private static ConnectionMultiplexer? _connection;

    private readonly DalOptions _dalSettings;

    public RedisProvider(IOptions<DalOptions> dalSettings)
    {
        _dalSettings = dalSettings.Value;
    }

    public async Task<IDatabase> GetConnection()
    {
        _connection ??= await ConnectionMultiplexer.ConnectAsync(_dalSettings.RedisConnectionString);

        return _connection.GetDatabase();
    }

}