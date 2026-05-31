using ArtifactStore.Application.Interfaces.Tenancy;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Infrastructure.Configurations;

namespace ArtifactStore.Infrastructure.Context;

public class ApplicationContext : DbContext, IApplicationContext
{
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

    private IConnectionStringHolder _connectionStringHolder;

    public ApplicationContext(DbContextOptions<ApplicationContext> options,
        IConnectionStringHolder connectionStringHolder) : base(options)
    {
        _connectionStringHolder = connectionStringHolder;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BalanceConfiguration());
        modelBuilder.ApplyConfiguration(new CurrencyConfiguration());
        modelBuilder.ApplyConfiguration(new ArtifactConfiguration());
        modelBuilder.ApplyConfiguration(new InventoryItemConfiguration());
        modelBuilder.ApplyConfiguration(new StoreItemConfiguration());
        modelBuilder.ApplyConfiguration(new CharacterConfiguration());
        modelBuilder.ApplyConfiguration(new StoreTransactionConfiguration());
        modelBuilder.ApplyConfiguration(new StoreTransactionTypeConfiguration());
        modelBuilder.ApplyConfiguration(new StoreTransactionStatusConfiguration());
        modelBuilder.ApplyConfiguration(new DiscountConfiguration());
    }
}