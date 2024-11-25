using SalesService.Domain.Interfaces;
using System.Threading.Channels;

namespace SalesService.DataAccess;

public class FileWriter : IFileWriter
{
    private readonly ChannelReader<string> _channel;
    private readonly string _filePath;
    private readonly IProfiler _profiler;

    public FileWriter(ChannelReader<string> channel, string filePath, IProfiler profiler)
    {
        _channel = channel;
        _filePath = filePath;
        _profiler = profiler;
    }

    public async Task WriteFileAsync(CancellationToken token)
    {
        using var streamWriter = new StreamWriter(_filePath);

        await streamWriter.WriteLineAsync("id, demand");

        try
        {
            await foreach (var line in _channel.ReadAllAsync(token))
            {
                if (token.IsCancellationRequested)
                    break;
                await streamWriter.WriteLineAsync(line);
                _profiler.IncrementWroteCount();
            }
        }
        catch (OperationCanceledException ex)
        {
            _profiler.LogMessage("WriteFileAsync caught cancel.");
        }
    }
}
