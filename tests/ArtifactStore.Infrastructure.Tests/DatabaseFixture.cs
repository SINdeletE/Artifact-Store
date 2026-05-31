using ArtifactStore.Application.Interfaces.Tenancy;
using ArtifactStore.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;

namespace ArtifactStore.Infrastructure.Tests;

public class DatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:18.3-alpine")
        .Build();

    public NpgsqlDataSource DataSource { get; private set; } = null!;
    public Guid SeedAccountId { get; private set; }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var dsBuilder = new NpgsqlDataSourceBuilder(_container.GetConnectionString());
        DataSource = dsBuilder.Build();

        await using var conn = await DataSource.OpenConnectionAsync();

        foreach (var file in new[] {"scripts/01_create.sql", 
                                          "scripts/02_procedures.sql",
                                          "scripts/03_constaints.sql",
                                          "scripts/05_seed_references.sql"
                 })
        {
            var sql = await File.ReadAllTextAsync(file);
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            await cmd.ExecuteNonQueryAsync();
        }

        SeedAccountId = Guid.NewGuid();
        await using var adminRoleCommand = conn.CreateCommand();
        adminRoleCommand.CommandText =
            "SELECT id FROM account_roles WHERE name = 'admin'";
        
        var resultExecute = await adminRoleCommand.ExecuteScalarAsync();
        var role = new Guid(resultExecute.ToString());
        
        await using var seedCmd = conn.CreateCommand();
        seedCmd.CommandText =
            "INSERT INTO accounts (id, role_id, nickname, password, email) VALUES ($1, $2, $3, $4, $5)";
        seedCmd.Parameters.AddWithValue(SeedAccountId);
        seedCmd.Parameters.AddWithValue(role);
        seedCmd.Parameters.AddWithValue("testuser");
        seedCmd.Parameters.AddWithValue("password123");
        seedCmd.Parameters.AddWithValue("test@test.com");
        await seedCmd.ExecuteNonQueryAsync();
    }

    public TestApplicationContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseNpgsql(DataSource)
            .Options;
        return new TestApplicationContext(options, new StubConnectionStringHolder());
    }

    public async Task DisposeAsync()
    {
        await DataSource.DisposeAsync();
        await _container.DisposeAsync();
    }
}

file sealed class StubConnectionStringHolder : IConnectionStringHolder
{
    public string GetConnectionString() => string.Empty;
}

[CollectionDefinition("Database")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>;