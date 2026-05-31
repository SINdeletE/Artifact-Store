using ArtifactStore.Domain.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ArtifactStore.Infrastructure.Context;

public interface IApplicationContext
{
    public DatabaseFacade Database { get; }
    public DbSet<Artifact> Artifacts { get; set; }
    public DbSet<Balance> Balances { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Character> Characters { get; set; }
    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<StoreItem> StoreItems { get; set; }
    public DbSet<StoreTransaction> StoreTransactions { get; set; }
    public DbSet<StoreTransactionStatus> StoreTransactionStatuses { get; set; }
    public DbSet<StoreTransactionType> StoreTransactionTypes { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}