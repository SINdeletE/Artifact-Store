using ArtifactStore.WebApp.Requires.Inventory;
using ArtifactStore.WebApp.Interfaces;
using ArtifactStore.WebApp.Exceptions;
using ArtifactStore.WebApp.Shared.Routes;

namespace ArtifactStore.WebApp.Endpoints;

public static class InventoryEndpoints
{
    private const string CurrentCharacterCookieName = "current_character_id";

    public static IEndpointRouteBuilder MapInventoryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGetCharacterInventoryPageEndpoint();
        endpoints.MapGetInventoryItemByIdEndpoint();
        endpoints.MapDeleteInventoryItemEndpoint();

        return endpoints;
    }

    public static IEndpointRouteBuilder MapGetCharacterInventoryPageEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(InventoryRoute.GetPage, async (
            [AsParameters] InventoryGetCurrentCharacterPageRequire req,
            HttpContext httpContext,
            IInventoryService inventoryService
        ) =>
        {
            if (!TryGetCurrentCharacterId(httpContext, out var characterId))
            {
                throw new ApiException("Current character is not selected.", StatusCodes.Status404NotFound);
            }

            var response = await inventoryService.GetInventoryPageByCharacterId(new InventoryGetPageByCharacterIdRequire
            {
                CharacterId = characterId,
                Page = req.Page,
                PageSize = req.PageSize
            });

            return Results.Ok(response);
        });

        return endpoints;
    }

    public static IEndpointRouteBuilder MapGetInventoryItemByIdEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(InventoryRoute.GetById, async (
            Guid id,
            IInventoryService inventoryService
        ) =>
        {
            var response = await inventoryService.GetInventoryItemById(id);

            return Results.Ok(response);
        });

        return endpoints;
    }

    public static IEndpointRouteBuilder MapDeleteInventoryItemEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete(InventoryRoute.Delete, async (
            Guid id,
            IInventoryService inventoryService
        ) =>
        {
            await inventoryService.DeleteInventoryItem(id);

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
