using ArtifactStore.Migration.CLI.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace ArtifactStore.Migration.CLI.Migrators;

public class PostgresToMongoMigrator
{
    private readonly MigrationDbContext _pg;
    private readonly MigrationMongoContext _mongo;

    public PostgresToMongoMigrator(MigrationDbContext pg, MigrationMongoContext mongo)
    {
        _pg = pg;
        _mongo = mongo;

        BatchSize = int.Parse(Environment.GetEnvironmentVariable("BATCH_SIZE") ?? "1000");
    }

    private readonly int BatchSize;

    public async Task RunAsync(CancellationToken ct = default)
    {
        Console.WriteLine("=== Postgres -> Mongo migration ===");

        Console.Write("  wiping mongo... ");
        await _mongo.WipeAsync(ct);
        Console.WriteLine("ok");

        await Copy(_pg.AccountRoles, _mongo.AccountRoles, "account_roles", ct);
        await Copy(_pg.Currencies, _mongo.Currencies, "currencies", ct);
        await Copy(_pg.Artifacts.IgnoreQueryFilters(), _mongo.Artifacts, "artifacts", ct);
        await Copy(_pg.Discounts.IgnoreQueryFilters(), _mongo.Discounts, "discounts", ct);
        await Copy(_pg.StoreTransactionTypes, _mongo.StoreTransactionTypes, "store_transaction_types", ct);
        await Copy(_pg.StoreTransactionStatuses, _mongo.StoreTransactionStatuses, "store_transaction_statuses", ct);
        await Copy(_pg.Accounts.IgnoreQueryFilters(), _mongo.Accounts, "accounts", ct);
        await Copy(_pg.Characters.IgnoreQueryFilters(), _mongo.Characters, "characters", ct);
        await Copy(_pg.Balances.IgnoreQueryFilters(), _mongo.Balances, "balances", ct);
        await Copy(_pg.InventoryItems, _mongo.InventoryItems, "inventory_items", ct);
        await Copy(_pg.StoreItems, _mongo.StoreItems, "store_items", ct);
        await Copy(_pg.StoreTransactions, _mongo.StoreTransactions, "store_transactions", ct);

        Console.WriteLine("=== Done ===");
    }

    private async Task Copy<T>(IQueryable<T> source, IMongoCollection<T> target,
        string name, CancellationToken ct) where T : class
    {
        Console.Write($"  {name}: ");

        var batch = new List<T>(BatchSize);
        var total = 0;

        // stream rows via the EF async enumerator instead of materializing the whole table
        await foreach (var row in source.AsNoTracking().AsAsyncEnumerable().WithCancellation(ct))
        {
            batch.Add(row);
            if (batch.Count < BatchSize)
                continue;

            await target.InsertManyAsync(batch, cancellationToken: ct);
            total += batch.Count;
            batch.Clear();
        }

        if (batch.Count > 0)
        {
            await target.InsertManyAsync(batch, cancellationToken: ct);
            total += batch.Count;
        }

        Console.WriteLine(total == 0 ? "0 (skipped)" : total.ToString());
    }
}
