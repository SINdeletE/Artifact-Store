using ArtifactStore.Application.Models.Requires.Store.Purchase;

namespace ArtifactStore.Application.Interfaces.Services.Store;

public interface IPurchaseService
{
    public Task PurchaseStoreItem(PurchaseStoreItemRequire req);
}