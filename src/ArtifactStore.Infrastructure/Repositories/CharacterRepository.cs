using ArtifactStore.Application.Interfaces.Repositories;
using ArtifactStore.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using Microsoft.EntityFrameworkCore.Storage;

namespace ArtifactStore.Infrastructure.Repositories;

public class CharacterRepository : ICharacterRepository
{
    IApplicationContext _dbContext;
    
    public CharacterRepository(IApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Character> GetByIdAsync(Guid id)
    {
        try
        {
            return await _dbContext.Characters
                .Where(c => c.Id == id)
                .FirstAsync();
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException("Character not found");
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

    public async Task<Character> InsertAsync(Character entity)
    {
        var entry = _dbContext.Characters.Add(entity);
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

    public async Task DeleteAsync(Character entity)
    {
        int rows = 0;
        
        try
        {
            rows = await _dbContext.Characters
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
            throw new NotFoundException("Character not found");
        }
    }

    public async Task UpdateAsync(Character entity)
    {
        int rows = 0;
        _dbContext.Characters.Update(entity);

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
            throw new NotFoundException("Character not found");
        }
    }

    public async Task<IEnumerable<Character>> FindAsync(BaseEntityFilter<Character> filter)
    {
        try
        {
            return await filter
                .Filter(_dbContext.Characters)
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