namespace ArtifactStore.Domain.Factories.Store;

public interface IStoreItemFactory
{
    public Entities.StoreItem Create(Entities.Currency currency, Entities.Artifact artifact,
        decimal price);
}