using ArtifactStore.Application.Models.Requires.StoreTransaction;
using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Interfaces.Services;

public interface IStoreTransactionService
{
    public Task<StoreTransaction> AddStoreTransaction(AddStoreTransactionRequire req);
    public Task<PageResult<StoreTransaction>> GetStoreTransactionPage(GetStoreTransactionPageRequire req);
}