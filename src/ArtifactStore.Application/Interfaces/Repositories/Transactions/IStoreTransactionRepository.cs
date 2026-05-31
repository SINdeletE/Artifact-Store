using ArtifactStore.Application.Interfaces.Repositories.Base;
using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Interfaces.Repositories.Transactions;

public interface IStoreTransactionRepository : IReadRepository<StoreTransaction>, 
    IWriteRepository<StoreTransaction>, IFilterRepository<StoreTransaction>
{
    // public Task<IEnumerable<StoreTransaction>> GetStoreTransactionPage(int page, int pageSize);
    // public Task AddStoreTransaction(StoreTransaction storeTransaction);
}