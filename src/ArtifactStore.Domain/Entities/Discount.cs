using ArtifactStore.Domain.Common;

namespace ArtifactStore.Domain.Entities;

public class Discount : IEntity
{
    public Guid Id { get; private set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Percent { get; set; }
    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public uint Version { get; set; }
    
    public Discount(){}
    public Discount(Guid id, string name, string description, decimal percent, DateTimeOffset startsAt,
        DateTimeOffset endsAt, DateTimeOffset? deletedAt)
    {
        Id = id;
        Name = name;
        Description = description;
        Percent = percent;
        StartsAt = startsAt;
        EndsAt = endsAt;
        DeletedAt = deletedAt;
    }
}