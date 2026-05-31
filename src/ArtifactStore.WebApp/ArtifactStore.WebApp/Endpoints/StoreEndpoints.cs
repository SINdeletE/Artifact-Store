using ArtifactStore.WebApp.Exceptions;
using ArtifactStore.WebApp.Requires.Store;
using ArtifactStore.WebApp.Interfaces;
using ArtifactStore.WebApp.Shared.Routes;

namespace ArtifactStore.WebApp.Endpoints;

public static class StoreEndpoints
{
    private const string CurrentCharacterCookieName = "current_character_id";

    public static IEndpointRouteBuilder MapStoreEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGetStoreItemPageEndpoint();
        endpoints.MapPurchaseStoreItemEndpoint();

        return endpoints;
    }

    public static IEndpointRouteBuilder MapGetStoreItemPageEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(StoreRoute.GetPage, async (
            [AsParameters] StoreGetItemPageRequire req,
            IStoreService storeService
        ) =>
        {
            var response = await storeService.GetStoreItemPage(req);

            return Results.Ok(response);
        });

        return endpoints;
    }

    public static IEndpointRouteBuilder MapPurchaseStoreItemEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(StoreRoute.Purchase, async (
            StorePurchaseItemRequire req,
            HttpContext httpContext,
            IStoreService storeService
        ) =>
        {
            if (!TryGetCurrentCharacterId(httpContext, out var characterId))
            {
                throw new ApiException("Current character is not selected.", StatusCodes.Status404NotFound);
            }

            req.CharacterId = characterId;
            await storeService.PurchaseStoreItem(req);

            return Results.Ok();
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
