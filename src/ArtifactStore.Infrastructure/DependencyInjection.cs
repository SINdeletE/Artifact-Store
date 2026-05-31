using ArtifactStore.Application.Interfaces.Repositories;
using ArtifactStore.Application.Interfaces.Repositories.Store;
using ArtifactStore.Application.Interfaces.Repositories.Transactions;
using ArtifactStore.Application.Interfaces.Tenancy;
using ArtifactStore.Infrastructure.Repositories;
using ArtifactStore.Infrastructure.Repositories.Mongo;
using ArtifactStore.Infrastructure.Repositories.Mongo.Store;
using ArtifactStore.Infrastructure.Repositories.Mongo.Transaction;
using ArtifactStore.Infrastructure.Repositories.Store;
using ArtifactStore.Infrastructure.Repositories.Transaction;
using ArtifactStore.Infrastructure.Strategy;
using ArtifactStore.Infrastructure.Tenancy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Npgsql;

namespace ArtifactStore.Infrastructure;

public static class DependencyInjection
{
    private static int _guidSerializerRegistered;

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        if (Interlocked.Exchange(ref _guidSerializerRegistered, 1) == 0)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        }

        // It's crazy, YEEAHHHHHH.
        var dataSources = new Dictionary<string, NpgsqlDataSource>();
        foreach (var connName in new[] { "DefaultUser", "Moderator", "Admin" })
        {
            var connStr = configuration.GetConnectionString(connName);
            if (connStr is null) continue;

            var builder = new NpgsqlDataSourceBuilder(connStr);
            dataSources[connName] = builder.Build();
        }
        services.AddSingleton<IReadOnlyDictionary<string, NpgsqlDataSource>>(dataSources);

        var dataSourcesMongoDb = new Dictionary<string, MongoClient>();
        foreach (var connName in new[] { "MongoDBDefaultUser", "MongoDBModerator", "MongoDBAdmin" })
        {
            var connStr = configuration.GetConnectionString(connName);
            if (connStr is null) continue;
            
            var settings = MongoClientSettings.FromConnectionString(connStr);
            settings.MaxConnectionPoolSize = 100;
            settings.MaxConnectionPoolSize = 5;
            settings.WaitQueueTimeout = TimeSpan.FromSeconds(30);
            
            dataSourcesMongoDb[connName] = new MongoClient(settings);
        }
        services.AddSingleton<IReadOnlyDictionary<string, MongoClient>>(dataSourcesMongoDb);
        
        services.AddScoped<IConnectionStringHolder, ConnectionStringHolder>();
        
        services.AddDbContext<ApplicationContext>((provider, options) =>
        {
            var holder = provider.GetRequiredService<IConnectionStringHolder>();
            options.UseNpgsql(dataSources[holder.GetConnectionString()],
                provider => provider.ExecutionStrategy(d => new RetryConnectionStrategy(d)));
        });

        services.AddKeyedScoped<IMongoDbContext>("Admin", (provider, _) =>
        {
            var clients = provider.GetRequiredService<IReadOnlyDictionary<string, MongoClient>>();
            var config = provider.GetRequiredService<IConfiguration>();
            var database = clients["MongoDBAdmin"]
                .GetDatabase(config["MONGODB_DB"] ?? "artifactstore");
            return new MongoDbContext(database);
        });
        services.AddScoped<IMongoDbContext>(provider =>
        {
            var clients = provider.GetRequiredService<IReadOnlyDictionary<string, MongoClient>>();
            var holder = provider.GetRequiredService<IConnectionStringHolder>();
            var config = provider.GetRequiredService<IConfiguration>();
            var database = clients["MongoDB" + holder.GetConnectionString()]
                .GetDatabase(config["MONGODB_DB"] ?? "artifactstore");
            return new MongoDbContext(database);
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });
        
        // For conflict resolution
        services.AddScoped<IApplicationContext>(provider => provider.GetRequiredService<ApplicationContext>());

        var db = configuration["CURRENT_DATABASE"] ?? "POSTGRES";
        if (db == "MONGO")
            services.AddMongoRepositories();
        else
            services.AddPostgresRepositories();

        return services;
    }

    private static IServiceCollection AddPostgresRepositories(this IServiceCollection services)
    {
        services.AddScoped<IArtifactRepository, ArtifactRepository>();
        services.AddScoped<IBalanceRepository, BalanceRepository>();
        services.AddScoped<ICharacterRepository, CharacterRepository>();
        services.AddScoped<ICurrencyRepository, CurrencyRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IStoreRepository, StoreRepository>();
        services.AddScoped<IStoreProcedureRepository, StoreProcedureRepository>();
        services.AddScoped<IStoreTransactionRepository, StoreTransactionRepository>();
        services.AddScoped<IStoreTransactionTypeRepository, StoreTransactionTypeRepository>();
        services.AddScoped<IStoreTransactionStatusRepository, StoreTransactionStatusRepository>();
        services.AddScoped<IDiscountRepository, DiscountRepository>();
        return services;
    }

    private static IServiceCollection AddMongoRepositories(this IServiceCollection services)
    {
        services.AddScoped<IArtifactRepository, MongoArtifactRepository>();
        services.AddScoped<IBalanceRepository, MongoBalanceRepository>();
        services.AddScoped<ICharacterRepository, MongoCharacterRepository>();
        services.AddScoped<ICurrencyRepository, MongoCurrencyRepository>();
        services.AddScoped<IInventoryRepository, MongoInventoryRepository>();
        services.AddScoped<IStoreRepository, MongoStoreRepository>();
        services.AddScoped<IStoreProcedureRepository, MongoStoreProcedureRepository>();
        services.AddScoped<IStoreTransactionRepository, MongoStoreTransactionRepository>();
        services.AddScoped<IStoreTransactionTypeRepository, MongoStoreTransactionTypeRepository>();
        services.AddScoped<IStoreTransactionStatusRepository, MongoStoreTransactionStatusRepository>();
        services.AddScoped<IDiscountRepository, MongoDiscountRepository>();
        return services;
    }
}
