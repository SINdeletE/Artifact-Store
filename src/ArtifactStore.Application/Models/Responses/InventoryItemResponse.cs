using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Models.Responses;

public class InventoryItemResponse
{
    public Guid Id { get; set; }
    
    public Guid CharacterId { get; set; }

    public Character Character
    {
        get;
        set
        {
            field = value;
            CharacterId = value.Id;
        }
    }

    public Guid ArtifactId { get; set; }

    public Artifact Artifact
    {
        get;
        set
        {
            field = value;
            ArtifactId = value.Id;
        }
    }

    public uint Version { get; set; }

    public InventoryItemResponse() {}
    public InventoryItemResponse(InventoryItem item)
    {
        Id = item.Id;
        Character = item.Character;
        Artifact = item.Artifact;
        Version = item.Version;
    }

    public InventoryItem Get()
    {
        return new InventoryItem(Id, Character, Artifact)
        {
            Version = Version
        };
    }
}