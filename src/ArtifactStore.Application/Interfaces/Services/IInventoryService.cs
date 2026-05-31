using ArtifactStore.Application.Models.Requires.Inventory;
using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Interfaces.Services;

public interface IInventoryService
{
    public Task DeleteInventoryItem(DeleteInventoryItemRequire req);
    public Task<PageResult<InventoryItem>> GetInventoryPageByCharacterId(GetInventoryPageByCharacterIdRequire req);
    public Task<InventoryItem> GetInventoryItemById(GetInventoryItemById req);
    public Task<InventoryItem> AddInventoryItem(AddInventoryItemRequire req);
}