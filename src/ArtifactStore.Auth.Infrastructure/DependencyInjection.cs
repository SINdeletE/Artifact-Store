using ArtifactStore.Auth.Application.Interfaces.Crypt;
using ArtifactStore.Auth.Application.Interfaces.Repositories.Account;
using ArtifactStore.Auth.Application.Interfaces.Tenancy;
using ArtifactStore.Auth.Application.Models.Crypt;
using ArtifactStore.Auth.Infrastructure.Repositories.Account;
using ArtifactStore.Auth.Infrastructure.Repositories.Mongo;
using ArtifactStore.Auth.Infrastructure.Strategy;
using ArtifactStore.Auth.Infrastructure.Tenancy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Npgsql;

namespace ArtifactStore.Auth.Infrastructure;

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
        
        // For conflict resolution
        services.AddScoped<IApplicationContext>(provider => provider.GetRequiredService<ApplicationContext>());
        
        var db = configuration["CURRENT_DATABASE"] ?? "POSTGRES";
        if (db == "MONGO")
            services.AddMongoRepositories();
        else
            services.AddPostgresRepositories();
        
        services.AddSingleton<IAccountPasswordCryptor, BCryptAccountPasswordCryptor>();
        
        return services;
    }
    
    private static IServiceCollection AddPostgresRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAccountRoleRepository, AccountRoleRepository>();
        services.AddScoped<IAccountRepository, AccountRepostiory>();
        
        return services;
    }

    private static IServiceCollection AddMongoRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAccountRoleRepository, MongoAccountRoleRepository>();
        services.AddScoped<IAccountRepository, MongoAccountRepository>();
        
        return services;
    }
}
