using ArtifactStore.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Application.Interfaces.Repositories.Store;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using Microsoft.EntityFrameworkCore.Storage;

namespace ArtifactStore.Infrastructure.Repositories.Store;

public class StoreRepository : IStoreRepository
{
    IApplicationContext _dbContext;
    
    public StoreRepository(IApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<StoreItem> GetByIdAsync(Guid id)
    {
        try
        {
            return await _dbContext.StoreItems
                .Where(i => i.Id == id)
                .Include(c => c.Currency)
                .Include(c => c.Artifact)
                .Include(c => c.Discount)
                .FirstAsync();
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException("Store item Not Found");
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

    public async Task<StoreItem> InsertAsync(StoreItem entity)
    {
        try
        {
            var entry = await _dbContext.StoreItems
                .AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entry.Entity;
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

    public async Task DeleteAsync(StoreItem entity)
    {
        int rows = 0;
        
        try
        {
            rows = await _dbContext.StoreItems
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
            throw new NotFoundException("Store item not found");
        }
    }

    public async Task UpdateAsync(StoreItem entity)
    {
        int rows = 0;
        _dbContext.StoreItems.Update(entity);

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
            throw new NotFoundException("Store item not found");
        }
    }

    public async Task<IEnumerable<StoreItem>> FindAsync(BaseEntityFilter<StoreItem> filter)
    {
        try
        {
            return await filter
                .Filter(_dbContext.StoreItems)
                .Include(c => c.Currency)
                .Include(c => c.Artifact)
                .Include(c => c.Discount)
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