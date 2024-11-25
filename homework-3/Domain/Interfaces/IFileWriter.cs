namespace SalesService.Domain.Interfaces;

public interface IFileWriter
{
    Task WriteFileAsync(CancellationToken token);
}
