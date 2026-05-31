using ArtifactStore.WebApp.Requires.StoreTransaction;
using ArtifactStore.WebApp.Responses;

namespace ArtifactStore.WebApp.Interfaces;

public interface IStoreTransactionService
{
    public Task<PageResponse<StoreTransactionResponse>> GetStoreTransactionPage(StoreTransactionGetPageRequire req);
}
