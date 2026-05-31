using ArtifactStore.Application.Interfaces.Repositories;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using Microsoft.EntityFrameworkCore.Storage;

namespace ArtifactStore.Infrastructure.Repositories;

public class DiscountRepository : IDiscountRepository
{
    private IApplicationContext _dbContext;

    public DiscountRepository(IApplicationContext context)
    {
        _dbContext = context;
    }

    public async Task<Discount> GetByIdAsync(Guid id)
    {
        try
        {
            return await _dbContext.Discounts
                .Where(c => c.Id == id)
                .FirstAsync();
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException("Discount not found");
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

    public async Task DeleteAsync(Discount entity)
    {
        int rows = 0;
        
        try
        {
            rows = await _dbContext.Discounts
                .Where(a => a.Id == entity.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(a => a.DeletedAt, DateTimeOffset.UtcNow));
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
            throw new NotFoundException("Discount not found");
        }
    }

    public async Task<Discount> InsertAsync(Discount entity)
    {
        var entry = _dbContext.Discounts.Add(entity);
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

    public async Task UpdateAsync(Discount entity)
    {
        int rows = 0;
        _dbContext.Discounts.Update(entity);

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
            throw new NotFoundException("Discount not found");
        }
    }
}