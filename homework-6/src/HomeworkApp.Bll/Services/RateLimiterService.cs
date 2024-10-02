using System;
using System.Threading;
using System.Threading.Tasks;
using HomeworkApp.Bll.Services.Interfaces;
using HomeworkApp.Dal.Infrastructure;
using HomeworkApp.Dal.Repositories;
using HomeworkApp.Dal.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace HomeworkApp.Bll.Services;

public class RateLimiterService : IRateLimiterService
{
    private readonly IRedisProvider _redisProvider;
    private readonly ILogger<RateLimiterService> _logger;
    private const int RequestsPerMinute = 100;
    private static readonly TimeSpan RequestsCountTtl = TimeSpan.FromMinutes(1);

    public RateLimiterService(
        IRedisProvider redisProvider,
        ILogger<RateLimiterService> logger)
    {
        _redisProvider = redisProvider;
        _logger = logger;
    }

    public async Task<bool> Allow(string clientId)
    {
        var redisDb = await _redisProvider.GetConnection();
        if (string.IsNullOrWhiteSpace(clientId))
        {
            return true;
        }

        try
        {
            await redisDb.StringSetAsync(clientId, RequestsPerMinute, RequestsCountTtl, when: When.NotExists);
            var exceeded = await redisDb.StringDecrementAsync(clientId) < 0;

            if (exceeded)
            {
                return false;
            }
        }
        catch (Exception ex) when (ex is not ApplicationException)
        {
            _logger.LogError(ex, "Не удалось прочитать/записать кеш");
        }

        return true;
    }
}