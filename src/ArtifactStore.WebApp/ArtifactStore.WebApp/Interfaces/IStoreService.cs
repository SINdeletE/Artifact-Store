using ArtifactStore.WebApp.Requires.Store;
using ArtifactStore.WebApp.Responses;

namespace ArtifactStore.WebApp.Interfaces;

public interface IStoreService
{
    public Task<StoreItemResponse> AddStoreItem(StoreAddItemRequire req);
    public Task DeleteStoreItem(Guid id);
    public Task UpdateStoreItem(StoreUpdateItemRequire req);
    public Task<PageResponse<StoreItemResponse>> GetStoreItemPage(StoreGetItemPageRequire req);
    public Task PurchaseStoreItem(StorePurchaseItemRequire req);
    public Task SellStoreItem(StoreSellItemRequire req);
}
