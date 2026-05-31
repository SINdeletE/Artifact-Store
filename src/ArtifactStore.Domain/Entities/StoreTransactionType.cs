using ArtifactStore.Domain.Common;

namespace ArtifactStore.Domain.Entities;

public class StoreTransactionType : IEntity
{
    public Guid Id { get; private set; }
    public string Name { get; set; }

    public StoreTransactionType(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}