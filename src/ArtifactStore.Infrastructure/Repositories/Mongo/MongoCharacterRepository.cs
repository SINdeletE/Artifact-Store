using ArtifactStore.Application.Interfaces.Repositories;
using ArtifactStore.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ArtifactStore.Infrastructure.Repositories.Mongo;

public class MongoCharacterRepository : ICharacterRepository
{
    IMongoDbContext _dbContext;

    public MongoCharacterRepository(IMongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Character> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _dbContext.Characters
                .Find(c => c.Id == id)
                .FirstOrDefaultAsync();
            if (entity is null)
                throw new NotFoundException("Character not found");
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

    public async Task<Character> InsertAsync(Character entity)
    {
        try
        {
            await _dbContext.Characters.InsertOneAsync(entity);
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

    public async Task DeleteAsync(Character entity)
    {
        UpdateResult result;

        try
        {
            var update = Builders<Character>.Update.Set(c => c.DeletedAt, DateTimeOffset.UtcNow);
            result = await _dbContext.Characters.UpdateOneAsync(c => c.Id == entity.Id, update);
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
            throw new NotFoundException("Character not found");
        }
    }

    public async Task UpdateAsync(Character entity)
    {
        ReplaceOneResult result;

        var currentVersion = entity.Version;
        var filter = Builders<Character>.Filter.And(
            Builders<Character>.Filter.Eq(c => c.Id, entity.Id),
            Builders<Character>.Filter.Eq(c => c.Version, currentVersion));
        entity.Version = currentVersion + 1;

        try
        {
            result = await _dbContext.Characters.ReplaceOneAsync(filter, entity);
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
            var exists = await _dbContext.Characters
                .Find(c => c.Id == entity.Id).AnyAsync();
            if (exists)
                throw new ConflictException("Concurrency conflict");
            throw new NotFoundException("Character not found");
        }
    }

    public async Task<IEnumerable<Character>> FindAsync(BaseEntityFilter<Character> filter)
    {
        try
        {
            return await MongoQueryable.ToListAsync(
                filter.Filter(_dbContext.Characters.AsQueryable()));
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
}
