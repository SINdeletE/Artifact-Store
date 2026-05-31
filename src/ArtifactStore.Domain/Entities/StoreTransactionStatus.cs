using ArtifactStore.Domain.Common;

namespace ArtifactStore.Domain.Entities;

public class StoreTransactionStatus : IEntity
{
    public Guid Id { get; private set; }
    public string Name { get; set; }

    public StoreTransactionStatus(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}