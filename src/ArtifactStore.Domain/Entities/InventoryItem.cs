using ArtifactStore.Domain.Common;

namespace ArtifactStore.Domain.Entities;

public class InventoryItem : IEntity
{
    public Guid Id { get; private set; }
    
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

    public InventoryItem() {}
    public InventoryItem(Guid id, Character character, Artifact artifact)
    {
        Id = id;
        Character = character;
        Artifact = artifact;
    }

    public InventoryItem(Guid id, Guid characterId, Guid artifactId)
    {
        Id = id;
        CharacterId = characterId;
        ArtifactId = artifactId;
    }
}