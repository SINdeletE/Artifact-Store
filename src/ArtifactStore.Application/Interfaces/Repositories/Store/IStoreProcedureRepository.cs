namespace ArtifactStore.Application.Interfaces.Repositories.Store;

public interface IStoreProcedureRepository
{
    public Task BuyAsync(Guid characterId, Guid storeItemId);
    public Task SellAsync(Guid inventoryItemId, Guid currencyId, decimal price);
}