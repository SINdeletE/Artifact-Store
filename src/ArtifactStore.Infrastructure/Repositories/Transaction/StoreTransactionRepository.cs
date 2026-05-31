using ArtifactStore.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Application.Interfaces.Repositories.Transactions;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using Microsoft.EntityFrameworkCore.Storage;

namespace ArtifactStore.Infrastructure.Repositories.Transaction;

public class StoreTransactionRepository : IStoreTransactionRepository
{
    IApplicationContext _dbContext;

    public StoreTransactionRepository(IApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StoreTransaction> GetByIdAsync(Guid id)
    {
        try
        {
            return await _dbContext.StoreTransactions
                .Where(c => c.Id == id)
                .Include(c => c.Balance)
                    .ThenInclude(b => b.Character)
                .Include(c => c.Balance)
                    .ThenInclude(b => b.Currency)
                .Include(c => c.Artifact)
                .Include(c => c.StoreTransactionType)
                .Include(c => c.StoreTransactionStatus)
                .FirstAsync();
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException("Store transaction not found");
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

    public async Task<StoreTransaction> InsertAsync(StoreTransaction entity)
    {
        var entry = _dbContext.StoreTransactions.Add(entity);
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (RetryLimitExceededException e)
        {
            throw new RetryConnectionException(e.Message);
        }
        catch (Exception e)
        {
            throw new ServerException(e.Message);
        }
        
        return entry.Entity;
    }

    public async Task DeleteAsync(StoreTransaction entity)
    {
        int rows = 0;
        
        try
        {
            rows = await _dbContext.StoreTransactions
                .Where(a => a.Id == entity.Id)
                .ExecuteDeleteAsync();
        }
        catch (RetryLimitExceededException e)
        {
            throw new RetryConnectionException(e.Message);
        }
        catch (Exception e)
        {
            throw new ServerException(e.Message);
        }

        if (rows == 0)
        {
            throw new NotFoundException("Store transaction not found");
        }
    }

    public async Task UpdateAsync(StoreTransaction entity)
    {
        int rows = 0;
        _dbContext.StoreTransactions.Update(entity);

        try
        {
            rows = await _dbContext.SaveChangesAsync();
        }
        catch (RetryLimitExceededException e)
        {
            throw new RetryConnectionException(e.Message);
        }
        catch (DbUpdateConcurrencyException e)
        {
            throw new ConflictException(e.Message);
        }
        catch (DbUpdateException e)
        {
            throw new ServerException(e.Message);
        }
        catch (Exception e)
        {
            throw new ServerException(e.Message);
        }

        if (rows == 0)
        {
            throw new NotFoundException("Store transaction not found");
        }
    }

    public async Task<IEnumerable<StoreTransaction>> FindAsync(BaseEntityFilter<StoreTransaction> filter)
    {
        try
        {
            return await filter
                .Filter(_dbContext.StoreTransactions)
                .Include(c => c.Balance)
                .ThenInclude(b => b.Character)
                .Include(c => c.Balance)
                .ThenInclude(b => b.Currency)
                .Include(c => c.Artifact)
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