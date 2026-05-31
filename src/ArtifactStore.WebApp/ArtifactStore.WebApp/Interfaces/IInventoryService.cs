using ArtifactStore.WebApp.Requires.Inventory;
using ArtifactStore.WebApp.Responses;

namespace ArtifactStore.WebApp.Interfaces;

public interface IInventoryService
{
    public Task DeleteInventoryItem(Guid id);
    public Task<InventoryItemResponse> GetInventoryItemById(Guid id);
    public Task<PageResponse<InventoryItemResponse>> GetInventoryPageByCharacterId(InventoryGetPageByCharacterIdRequire req);
    public Task<InventoryItemResponse> AddInventoryItem(InventoryAddItemRequire req);
}
