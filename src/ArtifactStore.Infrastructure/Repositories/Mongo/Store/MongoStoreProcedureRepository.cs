using ArtifactStore.Application.Interfaces.Repositories.Store;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace ArtifactStore.Infrastructure.Repositories.Mongo.Store;

public class MongoStoreProcedureRepository : IStoreProcedureRepository
{
    IMongoDbContext _dbContext;
    IMongoDbContext _adminDbContext;

    public MongoStoreProcedureRepository(IMongoDbContext dbContext,
        [FromKeyedServices("Admin")] IMongoDbContext adminDbContext)
    {
        _dbContext = dbContext;
        _adminDbContext = adminDbContext;
    }

    public async Task BuyAsync(Guid characterId, Guid storeItemId)
    {
        using var session = await _adminDbContext.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var storeItem = await _adminDbContext.StoreItems
                .Find(session, s => s.Id == storeItemId)
                .FirstOrDefaultAsync();
            if (storeItem is null)
                throw new NotFoundException("Store item not found");

            var balance = await _adminDbContext.Balances
                .Find(session, b => b.CharacterId == characterId
                                    && b.CurrencyId == storeItem.CurrencyId)
                .FirstOrDefaultAsync();
            if (balance is null)
                throw new NotFoundException("Balance not found");

            if (balance.Amount < storeItem.Price)
                throw new PaymentRequiredException("Not enough money");

            var deleteResult = await _adminDbContext.StoreItems
                .DeleteOneAsync(session, s => s.Id == storeItemId);
            if (deleteResult.DeletedCount == 0)
                throw new ConflictException("Data is not valid");

            var balanceFilter = Builders<Balance>.Filter.And(
                Builders<Balance>.Filter.Eq(b => b.Id, balance.Id),
                Builders<Balance>.Filter.Gte(b => b.Amount, storeItem.Price));
            var balanceUpdate = Builders<Balance>.Update.Inc(b => b.Amount, -storeItem.Price);
            var updateResult = await _adminDbContext.Balances
                .UpdateOneAsync(session, balanceFilter, balanceUpdate);
            if (updateResult.MatchedCount == 0)
                throw new ConflictException("Data is not valid");

            var inventoryItem = new InventoryItem(Guid.NewGuid(), characterId, storeItem.ArtifactId);
            await _adminDbContext.InventoryItems.InsertOneAsync(session, inventoryItem);

            await session.CommitTransactionAsync();
        }
        catch
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }

    public Task SellAsync(Guid inventoryItemId, Guid currencyId, decimal price)
    {
        return Task.CompletedTask;
    }
}
