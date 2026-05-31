using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Models.Requires.Store;

public class AddStoreItemRequire
{
    public Domain.Entities.Currency Currency { get; set; }
    public Domain.Entities.Artifact Artifact { get; set; }
    public decimal Price { get; set; }

    public AddStoreItemRequire(Domain.Entities.Currency currency, Domain.Entities.Artifact artifact, decimal price)
    {
        Currency = currency;
        Artifact = artifact;
        Price = price;
    }
}