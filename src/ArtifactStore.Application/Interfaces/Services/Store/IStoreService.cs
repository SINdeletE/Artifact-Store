using ArtifactStore.Application.Models.Requires.Store;
using ArtifactStore.Domain.Entities;
using PurchaseStoreItemRequire = ArtifactStore.Application.Models.Requires.Store.Purchase.PurchaseStoreItemRequire;
using SellStoreItemRequire = ArtifactStore.Application.Models.Requires.Store.Sell.SellStoreItemRequire;

namespace ArtifactStore.Application.Interfaces.Services.Store;

public interface IStoreService
{
    public Task PurchaseStoreItem(PurchaseStoreItemRequire req);
    public Task SellStoreItem(SellStoreItemRequire req);
    public Task<PageResult<StoreItem>> GetStoreItemPage(GetStoreItemPageRequire req);
    public Task<StoreItem> GetStoreItemById(Guid storeItemId);
    public Task  UpdateStoreItem(UpdateStoreItemRequire req);
    public Task<StoreItem> AddStoreItem(AddStoreItemRequire req);
    public Task DeleteStoreItem(DeleteStoreItemRequire req);
}