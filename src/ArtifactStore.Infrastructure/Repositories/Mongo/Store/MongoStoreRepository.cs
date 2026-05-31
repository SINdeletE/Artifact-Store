using ArtifactStore.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Application.Interfaces.Repositories.Store;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ArtifactStore.Infrastructure.Repositories.Mongo.Store;

public class MongoStoreRepository : IStoreRepository
{
    IMongoDbContext _dbContext;

    public MongoStoreRepository(IMongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StoreItem> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _dbContext.StoreItems
                .Find(i => i.Id == id)
                .FirstOrDefaultAsync();
            if (entity is null)
                throw new NotFoundException("Store item Not Found");

            await PopulateReferencesAsync(new[] { entity });
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

    public async Task<StoreItem> InsertAsync(StoreItem entity)
    {
        try
        {
            await _dbContext.StoreItems.InsertOneAsync(entity);
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

    public async Task DeleteAsync(StoreItem entity)
    {
        DeleteResult result;

        try
        {
            result = await _dbContext.StoreItems.DeleteOneAsync(i => i.Id == entity.Id);
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
            throw new NotFoundException("Store item not found");
        }
    }

    public async Task UpdateAsync(StoreItem entity)
    {
        ReplaceOneResult result;

        var currentVersion = entity.Version;
        var filter = Builders<StoreItem>.Filter.And(
            Builders<StoreItem>.Filter.Eq(i => i.Id, entity.Id),
            Builders<StoreItem>.Filter.Eq(i => i.Version, currentVersion));
        entity.Version = currentVersion + 1;

        try
        {
            result = await _dbContext.StoreItems.ReplaceOneAsync(filter, entity);
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
            var exists = await _dbContext.StoreItems
                .Find(i => i.Id == entity.Id).AnyAsync();
            if (exists)
                throw new ConflictException("Concurrency conflict");
            throw new NotFoundException("Store item not found");
        }
    }

    public async Task<IEnumerable<StoreItem>> FindAsync(BaseEntityFilter<StoreItem> filter)
    {
        try
        {
            var items = await MongoQueryable.ToListAsync(
                filter.Filter(_dbContext.StoreItems.AsQueryable()));
            await PopulateReferencesAsync(items);
            return items;
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

    private async Task PopulateReferencesAsync(IReadOnlyCollection<StoreItem> items)
    {
        if (items.Count == 0) return;

        var currencyIds = items.Select(i => i.CurrencyId).Distinct().ToList();
        var artifactIds = items.Select(i => i.ArtifactId).Distinct().ToList();
        var discountIds = items.Where(i => i.DiscountId.HasValue)
            .Select(i => i.DiscountId!.Value).Distinct().ToList();

        var currencies = (await _dbContext.Currencies.Find(c => currencyIds.Contains(c.Id))
            .ToListAsync()).ToDictionary(c => c.Id);
        var artifacts = (await _dbContext.Artifacts.Find(a => artifactIds.Contains(a.Id))
            .ToListAsync()).ToDictionary(a => a.Id);
        var discounts = discountIds.Count > 0
            ? (await _dbContext.Discounts.Find(d => discountIds.Contains(d.Id))
                .ToListAsync()).ToDictionary(d => d.Id)
            : new Dictionary<Guid, Discount>();

        foreach (var item in items)
        {
            if (currencies.TryGetValue(item.CurrencyId, out var currency))
                item.Currency = currency;
            if (artifacts.TryGetValue(item.ArtifactId, out var artifact))
                item.Artifact = artifact;
            if (item.DiscountId.HasValue && discounts.TryGetValue(item.DiscountId.Value, out var discount))
                item.Discount = discount;
        }
    }
}
