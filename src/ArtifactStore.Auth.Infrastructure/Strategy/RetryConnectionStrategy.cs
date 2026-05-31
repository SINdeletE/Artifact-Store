using Microsoft.EntityFrameworkCore.Storage;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal;

namespace ArtifactStore.Auth.Infrastructure.Strategy;

public class RetryConnectionStrategy : NpgsqlRetryingExecutionStrategy
{
    public RetryConnectionStrategy(ExecutionStrategyDependencies dependencies) : base(dependencies: dependencies) {}

    protected override TimeSpan? GetNextDelay(Exception lastException)
    {
        var retryList = ExceptionsEncountered;
        
        return TimeSpan.FromSeconds(5 * retryList.Count);
    }
}