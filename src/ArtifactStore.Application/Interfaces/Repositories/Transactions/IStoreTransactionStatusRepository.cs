using ArtifactStore.Application.Interfaces.Repositories.Base;
using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Interfaces.Repositories.Transactions;

public interface IStoreTransactionStatusRepository : IFilterRepository<StoreTransactionStatus>
{
    
}