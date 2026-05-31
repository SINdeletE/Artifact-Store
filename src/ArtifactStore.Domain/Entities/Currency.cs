using ArtifactStore.Domain.Common;

namespace ArtifactStore.Domain.Entities;

public class Currency : IEntity
{
    public Guid Id { get; private set; }
    public string Name { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public Currency() {}
    public Currency(Guid id, string name, DateTimeOffset? deletedAt)
    {
        Id = id;
        Name = name;
        DeletedAt = deletedAt;
    }
}