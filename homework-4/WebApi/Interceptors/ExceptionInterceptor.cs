using Grpc.Core;
using Grpc.Core.Interceptors;
using ProductService.Domain.Exceptions;
using ProductService.WebApi.Exceptions;

namespace ProductService.WebApi.Interceptors;

public class ExceptionInterceptor : Interceptor
{
    private readonly ILogger<ExceptionInterceptor> _logger;

    public ExceptionInterceptor(ILogger<ExceptionInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception thrown in gRPC call to {context.Method}: {ex}");

            throw HandleException(ex);
        }
    }

    private RpcException HandleException(Exception ex)
    {
        if (ex is BadRequestException)
            return new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));

        else if (ex is NotFoundException)
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));

        else if (ex is AlreadyExistsException)
            throw new RpcException(new Status(StatusCode.Aborted, "Id is already occupied"));

        else
            return new RpcException(new Status(StatusCode.Internal, "An internal error occurred."));
    }
}