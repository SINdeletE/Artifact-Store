using System.Security.Claims;
using ArtifactStore.WebApp.Interfaces;
using ArtifactStore.WebApp.Requires;
using ArtifactStore.WebApp.Requires.Auth;
using ArtifactStore.WebApp.Shared.Routes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace ArtifactStore.WebApp.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapAuthLoginEndpoint();
        endpoints.MapAuthRegisterEndpoint();
        
        return endpoints;
    }

    public static IEndpointRouteBuilder MapAuthLoginEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(AuthRoute.Login, async (
            LoginRequire req,
            IAccountService accountService,
            HttpContext httpContext
        ) =>
        {
            var loginResponse = await accountService.Login(req);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, req.Nickname),
                new Claim("access_token", loginResponse.Token),
            };
            
            var identity = new ClaimsIdentity(
                claims,
                IdentityConstants.ApplicationScheme);

            var principal = new ClaimsPrincipal(identity);
            
            await httpContext.SignInAsync(
                IdentityConstants.ApplicationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
                });
            
            return Results.Ok();
        });
        
        return endpoints;
    }
    
    public static IEndpointRouteBuilder MapAuthRegisterEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(AuthRoute.Register, async (
            RegisterRequire req,
            IAccountService accountService
        ) =>
        {
            await accountService.Register(req);
            
            return Results.Ok();
        });
        
        return endpoints;
    }
}
