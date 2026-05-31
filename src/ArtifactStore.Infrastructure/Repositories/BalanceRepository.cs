using ArtifactStore.Application.Interfaces.Repositories;
using ArtifactStore.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using Microsoft.EntityFrameworkCore.Storage;

namespace ArtifactStore.Infrastructure.Repositories;

public class BalanceRepository : IBalanceRepository
{
    IApplicationContext _dbContext;
    
    public BalanceRepository(IApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task UpdateAsync(Balance entity)
    {
        int rows = 0;
        _dbContext.Balances.Update(entity);

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
            throw new NotFoundException("Balance not found");
        }
    }

    public async Task<Balance> GetByIdAsync(Guid id)
    {
        try
        {
            return await _dbContext.Balances
                .Where(b => b.Id == id)
                .Include(c => c.Character)
                .Include(c => c.Currency)
                .FirstAsync();
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException("Balance not found");
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

    public async Task DeleteAsync(Balance entity)
    {
        int rows = 0;
        
        try
        {
            rows = await _dbContext.Balances
                .Where(b => b.Id == entity.Id)
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
            throw new NotFoundException("Balance not found");
        }
    }

    public async Task<Balance> InsertAsync(Balance entity)
    {
        try
        {
            var entry = await _dbContext.Balances.AddAsync(entity);
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

    public async Task<IEnumerable<Balance>> FindAsync(BaseEntityFilter<Balance> filter)
    {
        try
        {
            return await filter
                .Filter(_dbContext.Balances)
                .Include(c => c.Character)
                .Include(c => c.Currency)
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