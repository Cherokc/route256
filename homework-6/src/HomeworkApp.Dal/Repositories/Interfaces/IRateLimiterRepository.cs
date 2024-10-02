using System;
using System.Threading;
using System.Threading.Tasks;

namespace HomeworkApp.Dal.Repositories.Interfaces;

public interface IRateLimiterRepository
{
    Task<string> Get(string userIp, CancellationToken token);

    Task Set(string userIp, string json, TimeSpan? expiry, CancellationToken token);
}