namespace ArtifactStore.WebApp.Requires.Inventory;

public class InventoryGetPageByCharacterIdRequire
{
    public Guid CharacterId { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
