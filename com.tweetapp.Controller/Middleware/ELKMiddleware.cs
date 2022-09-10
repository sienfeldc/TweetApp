using ILogger = Serilog.ILogger;

namespace com.tweetapp.Controller.Middleware;

/// <summary>
/// 
/// </summary>
public class ELKMiddleware
{
    private readonly ILogger _logger;
    private readonly RequestDelegate _next;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="next"></param>
    /// <param name="logger"></param>
    public ELKMiddleware(RequestDelegate next, ILogger logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.Error(exception, "Something went wrong, {message}", exception.Data["someVeryImportantAttribute"]);

            await HandleExceptionAsync(context, exception);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = 500;
        return context.Response.WriteAsync("An exception occured");
    }
}