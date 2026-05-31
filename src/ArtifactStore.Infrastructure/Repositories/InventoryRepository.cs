using ArtifactStore.Application.Interfaces.Repositories;
using ArtifactStore.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using Microsoft.EntityFrameworkCore.Storage;

namespace ArtifactStore.Infrastructure.Repositories;

public class InventoryRepository : IInventoryRepository
{
    IApplicationContext _dbContext;
    
    public InventoryRepository(IApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<InventoryItem> GetByIdAsync(Guid id)
    {
        try
        {
            return await _dbContext.InventoryItems
                .Where(i => i.Id == id)
                .Include(i => i.Character)
                .Include(i => i.Artifact)
                .FirstAsync();
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException("Inventory item not found");
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

    public async Task<InventoryItem> InsertAsync(InventoryItem entity)
    {
        try
        {
            var entry = await _dbContext.InventoryItems.AddAsync(entity);
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

    public async Task DeleteAsync(InventoryItem entity)
    {
        int rows = 0;
        
        try
        {
            rows = await _dbContext.InventoryItems
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
            throw new NotFoundException("Inventory item not found");
        }
    }

    public async Task UpdateAsync(InventoryItem entity)
    {
        int rows = 0;
        _dbContext.InventoryItems.Update(entity);

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
            throw new NotFoundException("Inventory item not found");
        }
    }

    public async Task<IEnumerable<InventoryItem>> FindAsync(BaseEntityFilter<InventoryItem> filter)
    {
        try
        {
            return await filter
                .Filter(_dbContext.InventoryItems)
                .Include(i => i.Character)
                .Include(i => i.Artifact)
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