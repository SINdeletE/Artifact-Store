using ArtifactStore.Migration.CLI.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace ArtifactStore.Migration.CLI.Migrators;

public class MongoToPostgresMigrator
{
    private readonly MigrationMongoContext _mongo;
    private readonly MigrationDbContext _pg;

    public MongoToPostgresMigrator(MigrationMongoContext mongo, MigrationDbContext pg)
    {
        _mongo = mongo;
        _pg = pg;

        BatchSize = int.Parse(Environment.GetEnvironmentVariable("BATCH_SIZE") ?? "1000");
    }

    private static readonly string[] TablesInDependencyOrder =
    {
        // Children first (will be truncated first via CASCADE)
        "store_transactions", "store_items", "inventory_items", "balances",
        "accounts", "characters", "discounts", "artifacts", "currencies",
        "store_transaction_statuses", "store_transaction_types", "account_roles"
    };

    public async Task RunAsync(CancellationToken ct = default)
    {
        Console.WriteLine("=== Mongo -> Postgres migration ===");

        await WipeAsync(ct);

        // Independent reference tables
        await Copy(_mongo.AccountRoles, _pg.AccountRoles, "account_roles", ct);
        await Copy(_mongo.Currencies, _pg.Currencies, "currencies", ct);
        await Copy(_mongo.Artifacts, _pg.Artifacts, "artifacts", ct);
        await Copy(_mongo.Discounts, _pg.Discounts, "discounts", ct);
        await Copy(_mongo.StoreTransactionTypes, _pg.StoreTransactionTypes, "store_transaction_types", ct);
        await Copy(_mongo.StoreTransactionStatuses, _pg.StoreTransactionStatuses, "store_transaction_statuses", ct);

        // Accounts depend on AccountRoles
        await Copy(_mongo.Accounts, _pg.Accounts, "accounts", ct);
        // Characters depend on Accounts (FK characters.account_id)
        await Copy(_mongo.Characters, _pg.Characters, "characters", ct);

        // Tables depending on Characters/Currencies/Artifacts/Discounts
        await Copy(_mongo.Balances, _pg.Balances, "balances", ct);
        await Copy(_mongo.InventoryItems, _pg.InventoryItems, "inventory_items", ct);
        await Copy(_mongo.StoreItems, _pg.StoreItems, "store_items", ct);
        await Copy(_mongo.StoreTransactions, _pg.StoreTransactions, "store_transactions", ct);

        Console.WriteLine("=== Done ===");
    }

    private async Task WipeAsync(CancellationToken ct)
    {
        Console.Write("  wiping postgres... ");
        var joined = string.Join(", ", TablesInDependencyOrder);
        await _pg.Database.ExecuteSqlRawAsync($"TRUNCATE TABLE {joined} CASCADE", ct);
        Console.WriteLine("ok");
    }

    private readonly int BatchSize;

    private async Task Copy<T>(IMongoCollection<T> source, DbSet<T> target,
        string name, CancellationToken ct) where T : class
    {
        Console.Write($"  {name}: ");

        // disable change detection: we only insert, so the overhead is pure waste
        _pg.ChangeTracker.AutoDetectChangesEnabled = false;
        try
        {
            var batch = new List<T>(BatchSize);
            var total = 0;

            // stream via cursor instead of loading the whole collection into memory
            using var cursor = await source.Find(FilterDefinition<T>.Empty).ToCursorAsync(ct);
            while (await cursor.MoveNextAsync(ct))
            {
                foreach (var doc in cursor.Current)
                {
                    batch.Add(doc);
                    if (batch.Count < BatchSize)
                        continue;

                    await FlushAsync(target, batch, ct);
                    total += batch.Count;
                    batch.Clear();
                }
            }

            if (batch.Count > 0)
            {
                await FlushAsync(target, batch, ct);
                total += batch.Count;
            }

            Console.WriteLine(total == 0 ? "0 (skipped)" : total.ToString());
        }
        finally
        {
            _pg.ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }

    private async Task FlushAsync<T>(DbSet<T> target, List<T> batch, CancellationToken ct) where T : class
    {
        await target.AddRangeAsync(batch, ct);
        await _pg.SaveChangesAsync(ct);

        // drop the inserted entities so the tracker (and memory) stay clean across batches
        _pg.ChangeTracker.Clear();
    }
}
