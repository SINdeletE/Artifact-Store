using ArtifactStore.Application.Interfaces.Repositories;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using Microsoft.EntityFrameworkCore.Storage;

namespace ArtifactStore.Infrastructure.Repositories;

public class ArtifactRepository : IArtifactRepository
{
    IApplicationContext _dbContext;

    public ArtifactRepository(IApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Artifact> InsertAsync(Artifact entity)
    {
        try
        {
            var entry = await _dbContext.Artifacts.AddAsync(entity);
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

    public async Task UpdateAsync(Artifact entity)
    {
        int rows = 0;
        _dbContext.Artifacts.Update(entity);

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
            throw new NotFoundException("Artifact not found");
        }
    }

    public async Task<Artifact> GetByIdAsync(Guid id)
    {
        try
        {
            return await _dbContext.Artifacts
                .Where(a => a.Id == id)
                .FirstAsync();
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException("Artifact not found");
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

    public async Task DeleteAsync(Artifact entity)
    {
        int rows = 0;
        
        try
        {
            rows = await _dbContext.Artifacts
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
            throw new NotFoundException("Artifact not found");
        }
    }
}