using Grpc.Core;
using Grpc.Core.Interceptors;

namespace ProductService.WebApi.Interceptors;

public class LoggerInterceptor:Interceptor
{
    private readonly ILogger<LoggerInterceptor> _logger;

    public LoggerInterceptor(ILogger<LoggerInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        _logger.LogInformation($"gRPC call to {context.Method} with request: {request}");

        var response = await continuation(request, context);

        _logger.LogInformation($"gRPC response: {response}");

        return response;
    }
}