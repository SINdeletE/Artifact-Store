using ArtifactStore.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Application.Interfaces.Repositories.Transactions;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ArtifactStore.Infrastructure.Repositories.Mongo.Transaction;

public class MongoStoreTransactionStatusRepository : IStoreTransactionStatusRepository
{
    IMongoDbContext _dbContext;

    public MongoStoreTransactionStatusRepository(IMongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<StoreTransactionStatus>> FindAsync(BaseEntityFilter<StoreTransactionStatus> filter)
    {
        try
        {
            return await MongoQueryable.ToListAsync(
                filter.Filter(_dbContext.StoreTransactionStatuses.AsQueryable()));
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
