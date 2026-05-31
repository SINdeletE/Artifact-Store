using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Models.Requires.Store.Sell;

public class SellStoreItemRequire
{
    public Guid InventoryItemId { get; set; }
    public Guid CurrencyId { get; set; }
    public decimal Price { get; set; }
    
    public SellStoreItemRequire(Guid inventoryItemId, Guid currencyId, decimal price)
    {
        InventoryItemId = inventoryItemId;
        CurrencyId = currencyId;
        Price = price;
    }
}