using SalesService.DataAccess;
using Microsoft.Extensions.Configuration;
using SalesService.Infrastructure;
using System.Threading.Channels;

namespace SalesService.Cli;

internal static class Startup
{
    public static async Task InitializeCli()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var inputFile = configuration["FilePaths:InputFilePath"] ?? "input.txt";
        var outputFile = configuration["FilePaths:OutputFilePath"] ?? "output.txt";

        if (!int.TryParse(configuration["ThreadSettings:DegreeOfParallelismForInput"], out var inputChannelCapacity))
            inputChannelCapacity = Environment.ProcessorCount * 2; 

        if (!int.TryParse(configuration["ThreadSettings:DegreeOfParallelismForOutput"], out var outputChannelCapacity))
            outputChannelCapacity = Environment.ProcessorCount * 2; 

        if (!int.TryParse(configuration["ThreadSettings:DegreeOfParallelismForCalculations"], out var calculationDegree))
            calculationDegree = Environment.ProcessorCount;

        var inputChannel = Channel.CreateBounded<string>(inputChannelCapacity);
        var outputChannel = Channel.CreateBounded<string>(outputChannelCapacity);

        var profiler = new Profiler();

        var reader = new FileReader(inputChannel.Writer, inputFile, profiler);
        var calculator = new SalesCalculator(inputChannel.Reader, outputChannel.Writer, profiler, calculationDegree);
        var writer = new FileWriter(outputChannel.Reader, outputFile, profiler);

        var cts = new CancellationTokenSource();

        var cli = new Cli(reader, writer, calculator, profiler);

        await cli.Run(cts);
    }
}
