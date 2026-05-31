using ArtifactStore.Domain.Entities;
using MongoDB.Driver;

namespace ArtifactStore.Infrastructure.Context;

public interface IMongoDbContext
{
    IMongoCollection<Artifact> Artifacts { get; }
    IMongoCollection<Balance> Balances { get; }
    IMongoCollection<Currency> Currencies { get; }
    IMongoCollection<Character> Characters { get; }
    IMongoCollection<InventoryItem> InventoryItems { get; }
    IMongoCollection<StoreItem> StoreItems { get; }
    IMongoCollection<StoreTransaction> StoreTransactions { get; }
    IMongoCollection<StoreTransactionStatus> StoreTransactionStatuses { get; }
    IMongoCollection<StoreTransactionType> StoreTransactionTypes { get; }
    IMongoCollection<Discount> Discounts { get; }

    Task<IClientSessionHandle> StartSessionAsync(CancellationToken cancellationToken = default);
}