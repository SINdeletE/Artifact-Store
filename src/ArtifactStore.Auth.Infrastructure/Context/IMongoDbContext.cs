using ArtifactStore.Auth.Domain.Entities;
using MongoDB.Driver;

namespace ArtifactStore.Auth.Infrastructure.Context;

public interface IMongoDbContext
{
    IMongoCollection<Account> Accounts { get; }
    IMongoCollection<AccountRole> AccountRoles { get; }

    Task<IClientSessionHandle> StartSessionAsync(CancellationToken cancellationToken = default);
}