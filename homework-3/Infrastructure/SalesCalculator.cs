using SalesService.Domain;
using SalesService.Domain.Interfaces;
using System.Threading.Channels;

namespace SalesService.Infrastructure;

public class SalesCalculator : IDemandCalculator
{
    private readonly ChannelReader<string> _salesPredictions;
    private readonly ChannelWriter<string> _salesDemands;
    private readonly int _maxDegreeOfParallelism;
    private readonly IProfiler _profiler;

    public SalesCalculator(ChannelReader<string> salesReader, ChannelWriter<string> salesWriter, IProfiler profiler, int maxDegreeOfParallelism = 0)
    {
        _salesPredictions = salesReader;
        _salesDemands = salesWriter;
        _maxDegreeOfParallelism = maxDegreeOfParallelism <= 0 ? Environment.ProcessorCount : maxDegreeOfParallelism;
        _profiler = profiler;
    }

    public async Task CalculateDemandAsync(CancellationToken token)
    {
        var tasks = new List<Task>();

        for(int i = 0; i < _maxDegreeOfParallelism; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    await foreach (var line in _salesPredictions.ReadAllAsync(token))
                    {
                        if (token.IsCancellationRequested)
                            break;

                        try
                        {
                            var product = ValidateInputLine(line);
                            GetDemandForProduct(product);

                            await _salesDemands.WriteAsync($"{product.Id}, {product.Demand}", token);
                            _profiler.IncrementCalculatedCount();
                        }
                        catch (ArgumentException ex)
                        {
                            continue;
                        }
                    }
                }
                catch (OperationCanceledException ex)
                {
                    _profiler.LogMessage("CalculateDemandAsync caught cancel.");
                }
            }));
        }

        await Task.WhenAll(tasks);
        _salesDemands.Complete();
    }

    private void GetDemandForProduct(Product product)
    {
        var demand = 0;
        for (int i = 0; i < 10000000; i++)
            demand = product.Prediction - product.Stock;

        product.Demand = Math.Max(0,demand);
    }
    
    private Product ValidateInputLine(string line)
    {
        if(string.IsNullOrEmpty(line))
            throw new ArgumentNullException("Incorrect input line format");

        var inputs = line.Split(',');

        if(inputs.Length != 3) 
            throw new ArgumentException("Incorrect input line format");

        if (!int.TryParse(inputs[1], out int prediction))
            throw new ArgumentException("Incorrect prediction field format");

        if (!int.TryParse(inputs[2], out int stock))
            throw new ArgumentException("Incorrect stock field format");

        return new Product() 
        { 
            Id = inputs[0], 
            Prediction = prediction, 
            Stock = stock 
        };
    }
}
