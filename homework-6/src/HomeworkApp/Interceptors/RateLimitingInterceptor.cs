using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using HomeworkApp.Bll.Services.Interfaces;

namespace HomeworkApp.Interceptors;

public class RateLimitingInterceptor : Interceptor
{
    private const string ClientIpHeader = "X-R256-USER-IP";
    private readonly IRateLimiterService _rateLimiterService;

    public RateLimitingInterceptor(IRateLimiterService rateLimiterService)
    {
        _rateLimiterService = rateLimiterService;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var clientIp = context.RequestHeaders.Get(ClientIpHeader);

        if (clientIp is null)
        {
            throw new RpcException(new Status(StatusCode.FailedPrecondition,
                $"{ClientIpHeader} is required"));
        }

        if(!await _rateLimiterService.Allow(clientIp.Value))
        {
            throw new RpcException(new Status(StatusCode.ResourceExhausted, "Too many requests"));
        }

        return await continuation(request, context);
    }
}