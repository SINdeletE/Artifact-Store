using System.Diagnostics;

namespace ArtifactStore.Auth.API.Middleware;

public class LoggerMiddleware
{
    ILogger<LoggerMiddleware> _logger;
    
    RequestDelegate _next;

    public LoggerMiddleware(RequestDelegate next, ILogger<LoggerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            _logger.LogInformation("Request {Method} {Path} started", context.Request.Method, context.Request.Path);
            Stopwatch stopwatch = Stopwatch.StartNew();
            
            await _next(context);

            stopwatch.Stop();
            var errorMessage = context.Items.TryGetValue("ErrorMessage", out var msg) ? msg as string : null;
            if (errorMessage is not null)
            {
                var errorLevel = context.Items.TryGetValue("ErrorLevel", out var level) && level is LogLevel l
                    ? l
                    : LogLevel.Error;
                
                _logger.Log(errorLevel, "Request {Method} {Path} finished with code {StatusCode} in {ProcessTimeDuration} ms: {ErrorMessage}",
                    context.Request.Method, context.Request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds, errorMessage);
            }
            else
                _logger.LogInformation("Request {Method} {Path} finished with code {StatusCode} in {ProcessTimeDuration} ms",
                    context.Request.Method, context.Request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception e)
        {
            _logger.LogError("Request {Method} {Path} threw an exception: {ExceptionText}",
                context.Request.Method, context.Request.Path, e.Message);
        }
    }
}