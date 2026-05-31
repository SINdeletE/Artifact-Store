using ArtifactStore.WebApp.Exceptions;
using ArtifactStore.WebApp.Shared.Responses;

namespace ArtifactStore.WebApp.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ApiException e)
        {
            await Results.Json(new ErrorResponse
                {
                    Error = e.Message
                }, statusCode: e.StatusCode)
                .ExecuteAsync(context);
        }
    }
}