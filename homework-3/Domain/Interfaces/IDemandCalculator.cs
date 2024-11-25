namespace SalesService.Domain.Interfaces;

public interface IDemandCalculator
{
    Task CalculateDemandAsync(CancellationToken token);
}
