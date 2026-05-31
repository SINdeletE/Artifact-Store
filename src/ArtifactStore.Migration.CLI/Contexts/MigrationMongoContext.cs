using ArtifactStore.Domain.Entities;
using MongoDB.Driver;
using AuthAccount = ArtifactStore.Auth.Domain.Entities.Account;
using AuthAccountRole = ArtifactStore.Auth.Domain.Entities.AccountRole;

namespace ArtifactStore.Migration.CLI.Contexts;

public class MigrationMongoContext
{
    private readonly IMongoDatabase _db;

    public MigrationMongoContext(IMongoDatabase db)
    {
        _db = db;
    }

    public IMongoCollection<Artifact> Artifacts => _db.GetCollection<Artifact>("artifacts");
    public IMongoCollection<Balance> Balances => _db.GetCollection<Balance>("balances");
    public IMongoCollection<Currency> Currencies => _db.GetCollection<Currency>("currencies");
    public IMongoCollection<Character> Characters => _db.GetCollection<Character>("characters");
    public IMongoCollection<InventoryItem> InventoryItems => _db.GetCollection<InventoryItem>("inventory_items");
    public IMongoCollection<StoreItem> StoreItems => _db.GetCollection<StoreItem>("store_items");
    public IMongoCollection<StoreTransaction> StoreTransactions => _db.GetCollection<StoreTransaction>("store_transactions");
    public IMongoCollection<StoreTransactionStatus> StoreTransactionStatuses => _db.GetCollection<StoreTransactionStatus>("store_transaction_statuses");
    public IMongoCollection<StoreTransactionType> StoreTransactionTypes => _db.GetCollection<StoreTransactionType>("store_transaction_types");
    public IMongoCollection<Discount> Discounts => _db.GetCollection<Discount>("discounts");
    public IMongoCollection<AuthAccount> Accounts => _db.GetCollection<AuthAccount>("accounts");
    public IMongoCollection<AuthAccountRole> AccountRoles => _db.GetCollection<AuthAccountRole>("account_roles");

    public async Task WipeAsync(CancellationToken ct = default)
    {
        var names = await (await _db.ListCollectionNamesAsync(cancellationToken: ct)).ToListAsync(ct);
        foreach (var name in names)
            await _db.DropCollectionAsync(name, ct);
    }
}
