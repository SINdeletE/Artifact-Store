using ArtifactStore.Application.Interfaces.Repositories.Store;
using ArtifactStore.Domain.Exceptions;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;

namespace ArtifactStore.Infrastructure.Repositories.Store;

public class StoreProcedureRepository : IStoreProcedureRepository
{
    ApplicationContext _dbContext;
    
    public StoreProcedureRepository(ApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task BuyAsync(Guid characterId, Guid storeItemId)
    {
        try
        {
            await _dbContext.Database.ExecuteSqlRawAsync(
                "CALL buy_item(@storeItemId, @characterId)",
                new NpgsqlParameter("@storeItemId", storeItemId),
                new NpgsqlParameter("@characterId", characterId));
        }
        catch (RetryLimitExceededException e)
        {
            throw new RetryConnectionException(e.Message);
        }
        catch (PostgresException e)
        {
            throw e.MessageText switch
            {
                "STORE ITEM NOT FOUND" => new NotFoundException("Store item not found"),
                "BALANCE NOT FOUND" => new NotFoundException("Balance not found"),
                "BALANCE NOT ENOUGH" => new PaymentRequiredException("Not enough money"),
                "CONFLICT" => new ConflictException("Data is not valid"),
                _ => new ServerException(e.MessageText)
            };
        }
        catch (Exception e)
        {
            throw new ServerException(e.Message);
        }
    }

    public async Task SellAsync(Guid inventoryItemId, Guid currencyId, decimal price)
    {
        try
        {
            await _dbContext.Database.ExecuteSqlRawAsync(
                "CALL sell_item(@inventory_item_id, @currency_id, @price)", 
                new NpgsqlParameter("@inventory_item_id", inventoryItemId),
                new NpgsqlParameter("@currency_id", currencyId),
                new NpgsqlParameter("@price", price));
        }
        catch (RetryLimitExceededException e)
        {
            throw new RetryConnectionException(e.Message);
        }
        catch (PostgresException e)
        {
            throw e.MessageText switch
            {
                "INVENTORY ITEM NOT FOUND" => new NotFoundException("Inventory item not found"),
                "BALANCE NOT FOUND" => new NotFoundException("Balance not found"),
                "CONFLICT" => new ConflictException("Data is not valid"),
                _ => new ServerException("Unexpected error occured")
            };
        }
        catch (Exception e)
        {
            throw new ServerException(e.Message);
        }
    }
}
