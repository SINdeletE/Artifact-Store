using ArtifactStore.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Application.Interfaces.Repositories.Transactions;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ArtifactStore.Infrastructure.Repositories.Mongo.Transaction;

public class MongoStoreTransactionRepository : IStoreTransactionRepository
{
    IMongoDbContext _dbContext;

    public MongoStoreTransactionRepository(IMongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StoreTransaction> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _dbContext.StoreTransactions
                .Find(t => t.Id == id)
                .FirstOrDefaultAsync();
            if (entity is null)
                throw new NotFoundException("Store transaction not found");
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

    public async Task<StoreTransaction> InsertAsync(StoreTransaction entity)
    {
        try
        {
            await _dbContext.StoreTransactions.InsertOneAsync(entity);
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

    public async Task DeleteAsync(StoreTransaction entity)
    {
        DeleteResult result;

        try
        {
            result = await _dbContext.StoreTransactions.DeleteOneAsync(t => t.Id == entity.Id);
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
            throw new NotFoundException("Store transaction not found");
        }
    }

    public async Task UpdateAsync(StoreTransaction entity)
    {
        ReplaceOneResult result;

        try
        {
            result = await _dbContext.StoreTransactions.ReplaceOneAsync(t => t.Id == entity.Id, entity);
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
            throw new NotFoundException("Store transaction not found");
        }
    }

    public async Task<IEnumerable<StoreTransaction>> FindAsync(BaseEntityFilter<StoreTransaction> filter)
    {
        try
        {
            return await MongoQueryable.ToListAsync(
                filter.Filter(_dbContext.StoreTransactions.AsQueryable()));
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
