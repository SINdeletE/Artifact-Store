using ArtifactStore.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Application.Interfaces.Repositories.Transactions;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using Microsoft.EntityFrameworkCore.Storage;

namespace ArtifactStore.Infrastructure.Repositories.Transaction;

public class StoreTransactionTypeRepository : IStoreTransactionTypeRepository
{
    IApplicationContext _dbContext;

    public StoreTransactionTypeRepository(IApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<StoreTransactionType>> FindAsync(BaseEntityFilter<StoreTransactionType> filter)
    {
        try
        {
            return await filter
                .Filter(_dbContext.StoreTransactionTypes)
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