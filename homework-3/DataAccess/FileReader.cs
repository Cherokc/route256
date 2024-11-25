using SalesService.Domain.Interfaces;
using System.Threading.Channels;

namespace SalesService.DataAccess;

public class FileReader : IFileReader
{
    private readonly ChannelWriter<string> _channel;
    private readonly string _filePath;
    private readonly IProfiler _profiler;

    public FileReader(ChannelWriter<string> channel, string filePath, IProfiler profiler)
    {
        _channel = channel;
        _filePath = filePath;
        _profiler = profiler;

        if (!File.Exists(filePath))
            FileUtilities.ForceCreateFile(filePath);
    }

    public async Task ReadFromFileAsync(CancellationToken token)
    {
        using var streamReader = new StreamReader(_filePath);
        string line;

        try
        {
            while (!string.IsNullOrEmpty(line = await streamReader.ReadLineAsync(token)))
            {
                if (token.IsCancellationRequested)
                    break;
                await _channel.WriteAsync(line);  
                _profiler.IncrementReadCount();  
            }
        }
        catch (Exception ex)
        {
            _profiler.LogMessage("ReadFromFileAsync caught cancel.");
        }

        _channel.Complete();
    }
}
