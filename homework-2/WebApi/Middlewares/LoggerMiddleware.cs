namespace ProductService.WebApi.Middlewares;

public class LoggerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggerMiddleware> _logger;

    public LoggerMiddleware(ILogger<LoggerMiddleware> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("New request {0}", context.Request.Method);

        try
        {
            await _next(context);
        }
        catch(Exception ex)
        {
            _logger.LogError("Error occured: {0}", ex.Message);
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync($"Problem on server side {ex}");
        }
    }
}