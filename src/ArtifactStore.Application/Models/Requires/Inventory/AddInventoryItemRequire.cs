namespace ArtifactStore.Application.Models.Requires.Inventory;

public class AddInventoryItemRequire
{
    public Guid CharacterId { get; set; }
    public Guid ArtifactId { get; set; }
}