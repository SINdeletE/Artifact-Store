using ArtifactStore.Auth.Application.Interfaces.Identity;

namespace ArtifactStore.Auth.API.Middleware;

public class UserInfoCollectionMiddleware
{
    RequestDelegate _next;

    public UserInfoCollectionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IIdentityHolder roleHolder)
    {
        var role = context.User.FindFirst("role")?.Value;
        roleHolder.Role = role ?? "default_user";
        
        var accountIdString = context.User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        if (Guid.TryParse(accountIdString, out Guid accountId))
            roleHolder.AccountId = accountId;
        else
            roleHolder.AccountId = Guid.Empty;
     
        await _next(context);
    }
}