using ArtifactStore.Application.Interfaces.Tenancy;
using ArtifactStore.Domain.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace ArtifactStore.Infrastructure.Context;

public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IMongoDatabase database)
    {
        _database = database;
    }

    public IMongoCollection<Artifact> Artifacts => _database.GetCollection<Artifact>("artifacts");
    public IMongoCollection<Balance> Balances => _database.GetCollection<Balance>("balances");
    public IMongoCollection<Currency> Currencies => _database.GetCollection<Currency>("currencies");
    public IMongoCollection<Character> Characters => _database.GetCollection<Character>("characters");
    public IMongoCollection<InventoryItem> InventoryItems => _database.GetCollection<InventoryItem>("inventory_items");
    public IMongoCollection<StoreItem> StoreItems => _database.GetCollection<StoreItem>("store_items");
    public IMongoCollection<StoreTransaction> StoreTransactions => _database.GetCollection<StoreTransaction>("store_transactions");
    public IMongoCollection<StoreTransactionStatus> StoreTransactionStatuses => _database.GetCollection<StoreTransactionStatus>("store_transaction_statuses");
    public IMongoCollection<StoreTransactionType> StoreTransactionTypes => _database.GetCollection<StoreTransactionType>("store_transaction_types");
    public IMongoCollection<Discount> Discounts => _database.GetCollection<Discount>("discounts");

    public Task<IClientSessionHandle> StartSessionAsync(CancellationToken cancellationToken = default)
        => _database.Client.StartSessionAsync(cancellationToken: cancellationToken);
}