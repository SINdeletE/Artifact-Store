using ArtifactStore.Application.Interfaces.Repositories;
using ArtifactStore.Application.Interfaces.Repositories.Store;
using ArtifactStore.Application.Interfaces.Repositories.Transactions;
using Microsoft.EntityFrameworkCore.Storage;

namespace ArtifactStore.Infrastructure.Repositories.Store;

public class StoreUnitOfWork : IStoreUnitOfWork
{
    IApplicationContext _dbContext;
    IDbContextTransaction? _dbContextTransaction = null;
    
    public StoreUnitOfWork(IApplicationContext dbContext, IStoreRepository storeRepository,
        IBalanceRepository balanceRepository, IInventoryRepository inventoryRepository,
        IStoreTransactionRepository storeTransactionRepository)
    {
        _dbContext = dbContext; // Этот контекст должен принадлежать всем сущностям ниже
        StoreRepository = storeRepository;
        BalanceRepository = balanceRepository;
        InventoryRepository = inventoryRepository;
        StoreTransactionRepository = storeTransactionRepository;
    }
    
    public IStoreRepository StoreRepository { get; }
    public IBalanceRepository BalanceRepository { get; }
    public IInventoryRepository InventoryRepository { get; }
    public IStoreTransactionRepository StoreTransactionRepository { get; }

    public async Task BeginTransaction()
    {
        if (_dbContextTransaction is not null)
        {
            throw new InvalidOperationException("Transaction already started");
        }
        
        _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();
    }

    public async Task Commit()
    {
        await _dbContext.SaveChangesAsync(); // На всякий случай
        await _dbContextTransaction!.CommitAsync();
        await _dbContextTransaction.DisposeAsync();
        _dbContextTransaction = null;
    }

    public async Task Rollback()
    {
        await _dbContextTransaction!.RollbackAsync();
        await _dbContextTransaction.DisposeAsync();
        
        _dbContextTransaction = null;
    }
}