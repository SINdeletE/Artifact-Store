namespace ArtifactStore.WebApp.Responses;

public class InventoryItemResponse
{
    public Guid Id { get; set; }
    public Guid CharacterId { get; set; }
    public CharacterResponse? Character { get; set; }
    public Guid ArtifactId { get; set; }
    public ArtifactResponse? Artifact { get; set; }
    public uint Version { get; set; }
}
