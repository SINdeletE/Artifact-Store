using ArtifactStore.Auth.Infrastructure.Configurations;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using AuthAccount = ArtifactStore.Auth.Domain.Entities.Account;
using AuthAccountRole = ArtifactStore.Auth.Domain.Entities.AccountRole;

namespace ArtifactStore.Migration.CLI.Contexts;

public class MigrationDbContext : DbContext
{
    public DbSet<Artifact> Artifacts => Set<Artifact>();
    public DbSet<Balance> Balances => Set<Balance>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<StoreItem> StoreItems => Set<StoreItem>();
    public DbSet<StoreTransaction> StoreTransactions => Set<StoreTransaction>();
    public DbSet<StoreTransactionStatus> StoreTransactionStatuses => Set<StoreTransactionStatus>();
    public DbSet<StoreTransactionType> StoreTransactionTypes => Set<StoreTransactionType>();
    public DbSet<Discount> Discounts => Set<Discount>();
    public DbSet<AuthAccount> Accounts => Set<AuthAccount>();
    public DbSet<AuthAccountRole> AccountRoles => Set<AuthAccountRole>();

    public MigrationDbContext(DbContextOptions<MigrationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ArtifactConfiguration());
        modelBuilder.ApplyConfiguration(new BalanceConfiguration());
        modelBuilder.ApplyConfiguration(new CurrencyConfiguration());
        modelBuilder.ApplyConfiguration(new CharacterConfiguration());
        modelBuilder.ApplyConfiguration(new InventoryItemConfiguration());
        modelBuilder.ApplyConfiguration(new StoreItemConfiguration());
        modelBuilder.ApplyConfiguration(new StoreTransactionConfiguration());
        modelBuilder.ApplyConfiguration(new StoreTransactionStatusConfiguration());
        modelBuilder.ApplyConfiguration(new StoreTransactionTypeConfiguration());
        modelBuilder.ApplyConfiguration(new DiscountConfiguration());
        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new AccountRoleConfiguration());
    }
}
