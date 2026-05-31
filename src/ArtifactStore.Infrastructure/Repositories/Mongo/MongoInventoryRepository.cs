using ArtifactStore.Application.Interfaces.Repositories;
using ArtifactStore.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ArtifactStore.Infrastructure.Repositories.Mongo;

public class MongoInventoryRepository : IInventoryRepository
{
    IMongoDbContext _dbContext;

    public MongoInventoryRepository(IMongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<InventoryItem> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _dbContext.InventoryItems
                .Find(i => i.Id == id)
                .FirstOrDefaultAsync();
            if (entity is null)
                throw new NotFoundException("Inventory item not found");
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

    public async Task<InventoryItem> InsertAsync(InventoryItem entity)
    {
        try
        {
            await _dbContext.InventoryItems.InsertOneAsync(entity);
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

    public async Task DeleteAsync(InventoryItem entity)
    {
        DeleteResult result;

        try
        {
            result = await _dbContext.InventoryItems.DeleteOneAsync(i => i.Id == entity.Id);
        }
        catch (MongoConnectionException e)
        {
            throw new RetryConnectionException(e.Message);
        }
        catch (Exception e)
        {
            throw new ServerException(e.Message);
        }

        if (result.DeletedCount == 0)
        {
            throw new NotFoundException("Inventory item not found");
        }
    }

    public async Task UpdateAsync(InventoryItem entity)
    {
        ReplaceOneResult result;

        var currentVersion = entity.Version;
        var filter = Builders<InventoryItem>.Filter.And(
            Builders<InventoryItem>.Filter.Eq(i => i.Id, entity.Id),
            Builders<InventoryItem>.Filter.Eq(i => i.Version, currentVersion));
        entity.Version = currentVersion + 1;

        try
        {
            result = await _dbContext.InventoryItems.ReplaceOneAsync(filter, entity);
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
            var exists = await _dbContext.InventoryItems
                .Find(i => i.Id == entity.Id).AnyAsync();
            if (exists)
                throw new ConflictException("Concurrency conflict");
            throw new NotFoundException("Inventory item not found");
        }
    }

    public async Task<IEnumerable<InventoryItem>> FindAsync(BaseEntityFilter<InventoryItem> filter)
    {
        try
        {
            return await MongoQueryable.ToListAsync(
                filter.Filter(_dbContext.InventoryItems.AsQueryable()));
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
