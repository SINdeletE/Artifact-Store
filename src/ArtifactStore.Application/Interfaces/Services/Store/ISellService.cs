using ArtifactStore.Application.Models.Requires.Store.Sell;

namespace ArtifactStore.Application.Interfaces.Services.Store;

public interface ISellService
{
    public Task SellStoreItem(SellStoreItemRequire req);
}