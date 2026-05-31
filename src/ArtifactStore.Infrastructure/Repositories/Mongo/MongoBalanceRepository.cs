using ArtifactStore.Application.Interfaces.Repositories;
using ArtifactStore.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ArtifactStore.Infrastructure.Repositories.Mongo;

public class MongoBalanceRepository : IBalanceRepository
{
    IMongoDbContext _dbContext;

    public MongoBalanceRepository(IMongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task UpdateAsync(Balance entity)
    {
        ReplaceOneResult result;

        var currentVersion = entity.Version;
        var filter = Builders<Balance>.Filter.And(
            Builders<Balance>.Filter.Eq(b => b.Id, entity.Id),
            Builders<Balance>.Filter.Eq(b => b.Version, currentVersion));
        entity.Version = currentVersion + 1;

        try
        {
            result = await _dbContext.Balances.ReplaceOneAsync(filter, entity);
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
            var exists = await _dbContext.Balances
                .Find(b => b.Id == entity.Id).AnyAsync();
            if (exists)
                throw new ConflictException("Concurrency conflict");
            throw new NotFoundException("Balance not found");
        }
    }

    public async Task<Balance> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _dbContext.Balances
                .Find(b => b.Id == id)
                .FirstOrDefaultAsync();
            if (entity is null)
                throw new NotFoundException("Balance not found");
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

    public async Task DeleteAsync(Balance entity)
    {
        UpdateResult result;

        try
        {
            var update = Builders<Balance>.Update.Set(b => b.DeletedAt, DateTimeOffset.UtcNow);
            result = await _dbContext.Balances.UpdateOneAsync(b => b.Id == entity.Id, update);
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
            throw new NotFoundException("Balance not found");
        }
    }

    public async Task<Balance> InsertAsync(Balance entity)
    {
        try
        {
            await _dbContext.Balances.InsertOneAsync(entity);
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

    public async Task<IEnumerable<Balance>> FindAsync(BaseEntityFilter<Balance> filter)
    {
        try
        {
            return await MongoQueryable.ToListAsync(
                filter.Filter(_dbContext.Balances.AsQueryable()));
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
