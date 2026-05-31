using ArtifactStore.WebApp.Interfaces;
using ArtifactStore.WebApp.Requires.Character;
using ArtifactStore.WebApp.Shared.Routes;

namespace ArtifactStore.WebApp.Endpoints;

public static class CharacterEndpoints
{
    public static IEndpointRouteBuilder MapCharacterEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGetUserCharactersEndpoint();
        endpoints.MapAddCharacterEndpoint();
        endpoints.MapDeleteCharacterEndpoint();
        endpoints.MapChooseCharacter();
        
        return endpoints;
    }
    
    public static IEndpointRouteBuilder MapGetUserCharactersEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(CharacterRoute.GetUserAll, async (
            ICharacterService characterService
        ) =>
        {
            var response = await characterService.GetUserCharacters();
            
            return Results.Ok(response);
        });
        
        return endpoints;
    }

    public static IEndpointRouteBuilder MapAddCharacterEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(CharacterRoute.Add, async (
            CharacterAddRequire req,
            ICharacterService characterService
        ) =>
        {
            var response = await characterService.AddCharacter(req);

            return Results.Created(string.Empty, response);
        });

        return endpoints;
    }

    public static IEndpointRouteBuilder MapDeleteCharacterEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete(CharacterRoute.Delete, async (
            Guid id,
            ICharacterService characterService
        ) =>
        {
            await characterService.DeleteCharacter(id);

            return Results.Ok();
        });

        return endpoints;
    }
    
    public static IEndpointRouteBuilder MapChooseCharacter(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(CharacterRoute.Choose, (
            CharacterChooseRequire req,
            HttpContext httpContext
        ) =>
        {
            httpContext.Response.Cookies.Append(
                "current_character_id",
                req.CharacterId.ToString(),
                new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Path = "/",
                    Secure = httpContext.Request.IsHttps,
                    Expires = DateTimeOffset.UtcNow.AddDays(30)
                });

            return Results.Ok();
        });

        return endpoints;
    }
}
