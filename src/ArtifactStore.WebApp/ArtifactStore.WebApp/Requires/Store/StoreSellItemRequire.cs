namespace ArtifactStore.WebApp.Requires.Store;

public class StoreSellItemRequire
{
    public Guid InventoryItemId { get; set; }
    public Guid CurrencyId { get; set; }
    public decimal Price { get; set; }
}
