using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Domain.Factories.Inventory;

public class InventoryItemFactory : IInventoryItemFactory
{
    public InventoryItem Create(Entities.Character character, Entities.Artifact artifact)
    {
        return new InventoryItem(Guid.NewGuid(), character, artifact);
    }
}