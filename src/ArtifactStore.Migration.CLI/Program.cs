using System.CommandLine;
using ArtifactStore.Migration.CLI.Bson;
using ArtifactStore.Migration.CLI.Contexts;
using ArtifactStore.Migration.CLI.Migrators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace ArtifactStore.Migration.CLI;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        LoadDotEnv();

        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var pgConn = configuration.GetConnectionString("Admin")
            ?? throw new InvalidOperationException("ConnectionStrings__Admin is not set");
        var mongoConn = configuration.GetConnectionString("MongoDBAdmin")
            ?? throw new InvalidOperationException("ConnectionStrings__MongoDBAdmin is not set");
        var mongoDbName = configuration["MONGODB_DB"] ?? "artifactstore";

        BsonRegistration.Register();

        var pgToMongo = new Command("pg-to-mongo", "Copy all data from Postgres to MongoDB");
        pgToMongo.SetHandler(async () =>
        {
            await using var pg = CreatePgContext(pgConn);
            var mongo = CreateMongoContext(mongoConn, mongoDbName);
            var migrator = new PostgresToMongoMigrator(pg, mongo);
            await migrator.RunAsync();
        });

        var mongoToPg = new Command("mongo-to-pg", "Copy all data from MongoDB to Postgres");
        mongoToPg.SetHandler(async () =>
        {
            await using var pg = CreatePgContext(pgConn);
            var mongo = CreateMongoContext(mongoConn, mongoDbName);
            var migrator = new MongoToPostgresMigrator(mongo, pg);
            await migrator.RunAsync();
        });

        var migrate = new Command("migrate", "Data migration commands");
        migrate.AddCommand(pgToMongo);
        migrate.AddCommand(mongoToPg);

        var root = new RootCommand("ArtifactStore data migration CLI");
        root.AddCommand(migrate);

        return await root.InvokeAsync(args);
    }

    private static MigrationDbContext CreatePgContext(string connStr)
    {
        var options = new DbContextOptionsBuilder<MigrationDbContext>()
            .UseNpgsql(connStr)
            .Options;
        return new MigrationDbContext(options);
    }

    private static MigrationMongoContext CreateMongoContext(string connStr, string dbName)
    {
        var client = new MongoClient(connStr);
        return new MigrationMongoContext(client.GetDatabase(dbName));
    }

    private static void LoadDotEnv()
    {
        var dir = Directory.GetCurrentDirectory();
        while (dir is not null)
        {
            var candidate = Path.Combine(dir, ".env");
            if (File.Exists(candidate))
            {
                DotNetEnv.Env.Load(candidate);
                return;
            }
            dir = Path.GetDirectoryName(dir);
        }
    }
}
