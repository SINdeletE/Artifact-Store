using ArtifactStore.Domain.Exceptions;

namespace ArtifactStore.WebAPI.Middleware;

public class ExceptionHandlingMiddleware
{
    RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (RetryConnectionException ex)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            context.Items["ErrorMessage"] = ex.Message;
            context.Items["ErrorLevel"] = LogLevel.Error;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (ConflictException ex)
        {
            context.Response.StatusCode = 409;
            context.Response.ContentType = "application/json";
            context.Items["ErrorMessage"] = ex.Message;
            context.Items["ErrorLevel"] = LogLevel.Warning;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (InternalServerErrorException ex)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            context.Items["ErrorMessage"] = ex.Message;
            context.Items["ErrorLevel"] = LogLevel.Error;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (NotFoundException ex)
        {
            context.Response.StatusCode = 404;
            context.Response.ContentType = "application/json";
            context.Items["ErrorMessage"] = ex.Message;
            context.Items["ErrorLevel"] = LogLevel.Warning;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (PaymentRequiredException ex)
        {
            context.Response.StatusCode = 402;
            context.Response.ContentType = "application/json";
            context.Items["ErrorMessage"] = ex.Message;
            context.Items["ErrorLevel"] = LogLevel.Warning;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            context.Items["ErrorMessage"] = ex.Message;
            context.Items["ErrorLevel"] = LogLevel.Warning;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (UnauthorizedException ex)
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            context.Items["ErrorMessage"] = ex.Message;
            context.Items["ErrorLevel"] = LogLevel.Warning;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (BadRequestException ex)
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";
            context.Items["ErrorMessage"] = ex.Message;
            context.Items["ErrorLevel"] = LogLevel.Warning;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (ServerException ex)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            context.Items["ErrorMessage"] = ex.Message;
            context.Items["ErrorLevel"] = LogLevel.Error;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            context.Items["ErrorMessage"] = ex.Message;
            context.Items["ErrorLevel"] = LogLevel.Error;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
    }
}
