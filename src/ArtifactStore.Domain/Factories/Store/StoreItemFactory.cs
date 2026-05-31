using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Domain.Factories.Store;

public class StoreItemFactory : IStoreItemFactory
{
    public StoreItem Create(Currency currency, Entities.Artifact artifact, decimal price)
    {
        return new StoreItem(Guid.NewGuid(), currency, artifact, price);
    }
}