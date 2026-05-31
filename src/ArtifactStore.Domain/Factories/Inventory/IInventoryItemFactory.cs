namespace ArtifactStore.Domain.Factories.Inventory;

public interface IInventoryItemFactory
{
    public Entities.InventoryItem Create(Entities.Character character, Entities.Artifact artifact);
}