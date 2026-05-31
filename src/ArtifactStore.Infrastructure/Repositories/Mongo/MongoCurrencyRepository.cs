using ArtifactStore.Application.Interfaces.Repositories;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using MongoDB.Driver;

namespace ArtifactStore.Infrastructure.Repositories.Mongo;

public class MongoCurrencyRepository : ICurrencyRepository
{
    IMongoDbContext _dbContext;

    public MongoCurrencyRepository(IMongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Currency> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _dbContext.Currencies
                .Find(c => c.Id == id)
                .FirstOrDefaultAsync();
            if (entity is null)
                throw new NotFoundException("Currency not found");
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
}
