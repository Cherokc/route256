namespace SalesService.Domain.Interfaces;

public interface IProfiler
{
    void IncrementReadCount();
    void IncrementCalculatedCount();
    void IncrementWroteCount();
    void LogMessage(string message);
    Task HandleProgressAsync(CancellationToken token);
}