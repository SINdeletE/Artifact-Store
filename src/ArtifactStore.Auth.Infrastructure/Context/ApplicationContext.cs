using ArtifactStore.Auth.Application.Interfaces.Tenancy;
using ArtifactStore.Auth.Domain.Entities;
using ArtifactStore.Auth.Infrastructure.Configurations;

namespace ArtifactStore.Auth.Infrastructure.Context;

public class ApplicationContext : DbContext, IApplicationContext
{
    public DbSet<AccountRole> AccountRoles { get; set; }
    public DbSet<Account> Accounts { get; set; }
    
    private IConnectionStringHolder _connectionStringHolder;

    public ApplicationContext(DbContextOptions<ApplicationContext> options,
        IConnectionStringHolder connectionStringHolder) : base(options)
    {
        _connectionStringHolder = connectionStringHolder;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AccountRoleConfiguration());
        modelBuilder.ApplyConfiguration(new AccountConfiguration());
    }
}