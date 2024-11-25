using SalesService.Domain.Interfaces;

namespace SalesService.Cli;

internal class Cli
{
    private readonly IFileReader _fileReader;
    private readonly IFileWriter _fileWriter;
    private readonly IDemandCalculator _demandCalculator;
    private readonly IProfiler _profiler;

    public Cli(IFileReader fileReader, IFileWriter fileWriter, IDemandCalculator demandCalculator, IProfiler cliProfiler)
    {
        _fileReader = fileReader;
        _fileWriter = fileWriter;
        _demandCalculator = demandCalculator;
        _profiler = cliProfiler;
    }

    public async Task Run(CancellationTokenSource cancelTokenSource)
    {
        var token = cancelTokenSource.Token;

        var keyPressTask = MonitorKeyPress(cancelTokenSource);
        var profilerTask = _profiler.HandleProgressAsync(token);

        var salesTasks = new List<Task>()
        {
            _fileReader.ReadFromFileAsync(token),
            _demandCalculator.CalculateDemandAsync(token),
            _fileWriter.WriteFileAsync(token)
        };

        await Task.WhenAll(salesTasks);
        cancelTokenSource.Cancel();

        Console.WriteLine("Done.");
    }

    private async Task MonitorKeyPress(CancellationTokenSource cts)
    {
        Console.WriteLine("Press C to stop.");

        await Task.Run(() =>
        {
            while (!cts.Token.IsCancellationRequested)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true);
                    if (key.Key == ConsoleKey.C)
                    {
                        Console.WriteLine("Abort requested.");
                        cts.Cancel();
                        break;
                    }
                }
            }
        });
    }
}
