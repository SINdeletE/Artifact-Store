using ArtifactStore.Domain.Common;

namespace ArtifactStore.Domain.Entities;

public class Artifact : IEntity
{
    public Guid Id { get; private set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public uint Version { get; set; }
    
    public Artifact() {}
    public Artifact(Guid id, string name, string description, DateTimeOffset? deletedAt)
    {
        Id = id;
        Name = name;
        Description = description;
        DeletedAt = deletedAt;
    }
}