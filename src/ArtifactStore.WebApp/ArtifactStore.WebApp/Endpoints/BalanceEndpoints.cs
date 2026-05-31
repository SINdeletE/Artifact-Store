using ArtifactStore.WebApp.Exceptions;
using ArtifactStore.WebApp.Requires.Balance;
using ArtifactStore.WebApp.Interfaces;
using ArtifactStore.WebApp.Shared.Routes;

namespace ArtifactStore.WebApp.Endpoints;

public static class BalanceEndpoints
{
    private const string CurrentCharacterCookieName = "current_character_id";

    public static IEndpointRouteBuilder MapBalanceEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGetCharacterBalancesEndpoint();

        return endpoints;
    }

    public static IEndpointRouteBuilder MapGetCharacterBalancesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(BalanceRoute.GetCharacterBalances, async (
            HttpContext httpContext,
            IBalanceService balanceService
        ) =>
        {
            if (!TryGetCurrentCharacterId(httpContext, out var characterId))
            {
                throw new ApiException("Current character is not selected.", StatusCodes.Status404NotFound);
            }

            var response = await balanceService.GetCharacterBalances(new CharacterBalancesRequire
            {
                CharacterId = characterId
            });

            return Results.Ok(response);
        });

        return endpoints;
    }

    private static bool TryGetCurrentCharacterId(HttpContext httpContext, out Guid characterId)
    {
        characterId = Guid.Empty;

        return httpContext.Request.Cookies.TryGetValue(CurrentCharacterCookieName, out var cookieValue)
            && Guid.TryParse(cookieValue, out characterId);
    }
}
