using ArtifactStore.Application.Interfaces.Repositories;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using MongoDB.Driver;

namespace ArtifactStore.Infrastructure.Repositories.Mongo;

public class MongoDiscountRepository : IDiscountRepository
{
    private IMongoDbContext _dbContext;

    public MongoDiscountRepository(IMongoDbContext context)
    {
        _dbContext = context;
    }

    public async Task<Discount> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _dbContext.Discounts
                .Find(d => d.Id == id)
                .FirstOrDefaultAsync();
            if (entity is null)
                throw new NotFoundException("Discount not found");
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

    public async Task DeleteAsync(Discount entity)
    {
        UpdateResult result;

        try
        {
            var update = Builders<Discount>.Update.Set(d => d.DeletedAt, DateTimeOffset.UtcNow);
            result = await _dbContext.Discounts.UpdateOneAsync(d => d.Id == entity.Id, update);
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
            throw new NotFoundException("Discount not found");
        }
    }

    public async Task<Discount> InsertAsync(Discount entity)
    {
        try
        {
            await _dbContext.Discounts.InsertOneAsync(entity);
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

    public async Task UpdateAsync(Discount entity)
    {
        ReplaceOneResult result;

        var currentVersion = entity.Version;
        var filter = Builders<Discount>.Filter.And(
            Builders<Discount>.Filter.Eq(d => d.Id, entity.Id),
            Builders<Discount>.Filter.Eq(d => d.Version, currentVersion));
        entity.Version = currentVersion + 1;

        try
        {
            result = await _dbContext.Discounts.ReplaceOneAsync(filter, entity);
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
            var exists = await _dbContext.Discounts
                .Find(d => d.Id == entity.Id).AnyAsync();
            if (exists)
                throw new ConflictException("Concurrency conflict");
            throw new NotFoundException("Discount not found");
        }
    }
}
