namespace SalesService.Domain.Interfaces;

public interface IFileReader
{
    Task ReadFromFileAsync(CancellationToken token);
}
