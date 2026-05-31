using ArtifactStore.Domain.Common;

namespace ArtifactStore.Domain.Entities;

public class Character : IEntity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public DateOnly CreationDate { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public uint Version { get; set; }

    public Character() {}
    public Character(Guid id, Guid userId, string name, DateOnly creationDate, DateTimeOffset? deletedAt)
    {
        Id = id;
        UserId = userId;
        Name = name;
        CreationDate = creationDate;
        DeletedAt = deletedAt;
    }
}