using SalesService.Domain.Interfaces;

namespace SalesService.Cli;

internal class Profiler : IProfiler
{
    private int _readCount;
    private int _wroteCount;
    private int _calculatedCount;

    public void IncrementReadCount()
    {
        Interlocked.Increment(ref _readCount);
    }

    public void IncrementCalculatedCount()
    {
        Interlocked.Increment(ref _calculatedCount);
    }

    public void IncrementWroteCount()
    {
        Interlocked.Increment(ref _wroteCount);
    }

    public async Task HandleProgressAsync(CancellationToken token)
    {
        Console.WriteLine("Read\tCalculated\tWrote");
        while (true)
        {
            if (token.IsCancellationRequested)
                break;

            var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffff");
            Console.WriteLine("{0}\t{1}\t\t{2}\t[{3}]", _readCount, _calculatedCount, _wroteCount, dateTime);

            await Task.Delay(50);
        }
    }

    public void LogMessage(string message)
    {
        Console.WriteLine(message);
    }
}