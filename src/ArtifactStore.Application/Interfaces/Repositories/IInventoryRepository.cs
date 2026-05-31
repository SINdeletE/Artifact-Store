using ArtifactStore.Application.Interfaces.Repositories.Base;
using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Interfaces.Repositories;

public interface IInventoryRepository : IReadRepository<InventoryItem>, IWriteRepository<InventoryItem>, IFilterRepository<InventoryItem>
{
    // public Task AddInventoryItem(InventoryItem inventoryItem);
    // public Task<InventoryItem?> GetInventoryItemById(Guid id);
    // public Task<IEnumerable<InventoryItem>> GetInventoryPageByUserId(Guid userId, int page, int pageSize);
    // public Task<bool> DeleteInventoryItem(Guid id);
}