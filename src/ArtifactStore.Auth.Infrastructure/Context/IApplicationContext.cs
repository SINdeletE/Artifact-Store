using ArtifactStore.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ArtifactStore.Auth.Infrastructure.Context;

public interface IApplicationContext
{
    public DatabaseFacade Database { get; }
    public DbSet<AccountRole> AccountRoles { get; set; }
    public DbSet<Account> Accounts { get; set; }
    
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}