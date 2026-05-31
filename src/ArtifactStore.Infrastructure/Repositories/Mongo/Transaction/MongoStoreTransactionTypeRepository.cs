using ArtifactStore.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Application.Interfaces.Repositories.Transactions;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ArtifactStore.Infrastructure.Repositories.Mongo.Transaction;

public class MongoStoreTransactionTypeRepository : IStoreTransactionTypeRepository
{
    IMongoDbContext _dbContext;

    public MongoStoreTransactionTypeRepository(IMongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<StoreTransactionType>> FindAsync(BaseEntityFilter<StoreTransactionType> filter)
    {
        try
        {
            return await MongoQueryable.ToListAsync(
                filter.Filter(_dbContext.StoreTransactionTypes.AsQueryable()));
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
