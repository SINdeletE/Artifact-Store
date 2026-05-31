using ArtifactStore.Application.Interfaces.Repositories;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using MongoDB.Driver;

namespace ArtifactStore.Infrastructure.Repositories.Mongo;

public class MongoArtifactRepository : IArtifactRepository
{
    IMongoDbContext _dbContext;

    public MongoArtifactRepository(IMongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Artifact> InsertAsync(Artifact entity)
    {
        try
        {
            await _dbContext.Artifacts.InsertOneAsync(entity);
            return entity;
        }
        catch (MongoConnectionException e)
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
        ReplaceOneResult result;

        var currentVersion = entity.Version;
        var filter = Builders<Artifact>.Filter.And(
            Builders<Artifact>.Filter.Eq(a => a.Id, entity.Id),
            Builders<Artifact>.Filter.Eq(a => a.Version, currentVersion));
        entity.Version = currentVersion + 1;

        try
        {
            result = await _dbContext.Artifacts.ReplaceOneAsync(filter, entity);
        }
        catch (MongoConnectionException e)
        {
            throw new RetryConnectionException(e.Message);
        }
        catch (Exception e)
        {
            throw new ServerException(e.Message);
        }

        if (result.MatchedCount == 0)
        {
            var exists = await _dbContext.Artifacts
                .Find(a => a.Id == entity.Id).AnyAsync();
            if (exists)
                throw new ConflictException("Concurrency conflict");
            throw new NotFoundException("Artifact not found");
        }
    }

    public async Task<Artifact> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _dbContext.Artifacts
                .Find(a => a.Id == id)
                .FirstOrDefaultAsync();
            if (entity is null)
                throw new NotFoundException("Artifact not found");
            return entity;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (MongoConnectionException e)
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
        UpdateResult result;

        try
        {
            var update = Builders<Artifact>.Update.Set(a => a.DeletedAt, DateTimeOffset.UtcNow);
            result = await _dbContext.Artifacts.UpdateOneAsync(a => a.Id == entity.Id, update);
        }
        catch (MongoConnectionException e)
        {
            throw new RetryConnectionException(e.Message);
        }
        catch (Exception e)
        {
            throw new ServerException(e.Message);
        }

        if (result.MatchedCount == 0)
        {
            throw new NotFoundException("Artifact not found");
        }
    }
}
