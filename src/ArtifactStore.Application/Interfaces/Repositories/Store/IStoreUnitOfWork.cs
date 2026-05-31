using ArtifactStore.Application.Interfaces.Repositories.Transactions;

namespace ArtifactStore.Application.Interfaces.Repositories.Store;

public interface IStoreUnitOfWork
{
    public IStoreTransactionRepository StoreTransactionRepository { get; }
    public IInventoryRepository InventoryRepository { get; }
    public IStoreRepository StoreRepository { get; }
    public IBalanceRepository BalanceRepository { get; }

    public Task BeginTransaction();
    public Task Commit();
    public Task Rollback();
}