using ArtifactStore.Auth.Domain.Entities;
using MongoDB.Driver;

namespace ArtifactStore.Auth.Infrastructure.Context;

public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IMongoDatabase database)
    {
        _database = database;
    }

    public IMongoCollection<Account> Accounts => _database.GetCollection<Account>("accounts");
    public IMongoCollection<AccountRole> AccountRoles => _database.GetCollection<AccountRole>("account_roles");

    public Task<IClientSessionHandle> StartSessionAsync(CancellationToken cancellationToken = default)
        => _database.Client.StartSessionAsync(cancellationToken: cancellationToken);
}