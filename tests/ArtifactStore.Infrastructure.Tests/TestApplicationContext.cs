using ArtifactStore.Application.Interfaces.Tenancy;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace ArtifactStore.Infrastructure.Tests;

public class TestApplicationContext : ApplicationContext
{
    public TestApplicationContext(DbContextOptions<ApplicationContext> options, IConnectionStringHolder holder)
        : base(options, holder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Artifact>().Ignore(e => e.Version);
        modelBuilder.Entity<Character>().Ignore(e => e.Version);
        modelBuilder.Entity<Balance>().Ignore(e => e.Version);
        modelBuilder.Entity<InventoryItem>().Ignore(e => e.Version);
        modelBuilder.Entity<StoreItem>().Ignore(e => e.Version);
    }
}
