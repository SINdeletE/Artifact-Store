namespace ArtifactStore.Application.Models.Requires.Store.Purchase;

public class PurchaseStoreItemRequire
{
    public Guid StoreItemId { get; set; }
    public Guid CharacterId { get; set; }

    public PurchaseStoreItemRequire()
    {
    }

    public PurchaseStoreItemRequire(Guid storeItemId, Guid characterId)
    { 
        StoreItemId = storeItemId;
        CharacterId = characterId;
    }
}
