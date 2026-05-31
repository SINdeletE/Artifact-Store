namespace ArtifactStore.Application.Models.Requires.Inventory;

public class GetInventoryPageByCharacterIdRequire
{
    public Guid CharacterId { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}