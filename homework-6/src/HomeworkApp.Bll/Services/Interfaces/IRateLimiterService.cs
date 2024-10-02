using System.Threading.Tasks;

namespace HomeworkApp.Bll.Services.Interfaces;

public interface IRateLimiterService
{
    Task<bool> Allow(string clientId);
}