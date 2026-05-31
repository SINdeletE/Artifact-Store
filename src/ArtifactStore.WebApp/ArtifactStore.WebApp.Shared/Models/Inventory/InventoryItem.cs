using ArtifactStore.WebApp.Shared.Models.Artifact;

namespace ArtifactStore.WebApp.Shared.Models.Inventory;

public sealed class InventoryItem
{
    public Guid Id { get; set; }
    public Guid CharacterId { get; set; }
    public Guid ArtifactId { get; set; }
    public Artifact.Artifact? Artifact { get; set; }
    public uint Version { get; set; }
}
