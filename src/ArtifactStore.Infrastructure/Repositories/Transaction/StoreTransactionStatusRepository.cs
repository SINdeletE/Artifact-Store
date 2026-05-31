using ArtifactStore.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Application.Interfaces.Repositories.Transactions;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using Microsoft.EntityFrameworkCore.Storage;

namespace ArtifactStore.Infrastructure.Repositories.Transaction;

public class StoreTransactionStatusRepository : IStoreTransactionStatusRepository
{
    IApplicationContext _dbContext;

    public StoreTransactionStatusRepository(IApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<StoreTransactionStatus>> FindAsync(BaseEntityFilter<StoreTransactionStatus> filter)
    {
        try
        {
            return await filter
                .Filter(_dbContext.StoreTransactionStatuses)
                .ToListAsync();
        }
        catch (RetryLimitExceededException e)
        {
            throw new RetryConnectionException(e.Message);
        }
        catch (Exception e)
        {
            throw new ServerException(e.Message);
        }
    }
}