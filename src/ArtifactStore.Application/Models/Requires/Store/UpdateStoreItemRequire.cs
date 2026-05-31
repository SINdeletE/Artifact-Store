using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Models.Requires.Store;

public class UpdateStoreItemRequire
{
    public Guid Id { get; set; }
    public Domain.Entities.Currency Currency { get; set; }
    public Domain.Entities.Artifact Artifact { get; set; }
    public decimal Price { get; set; }
    public uint Version { get; set; }

    public UpdateStoreItemRequire(Guid id, Domain.Entities.Currency currency,
        Domain.Entities.Artifact artifact, decimal price, uint version)
    {
        Id = id;
        Currency = currency;
        Artifact = artifact;
        Price = price;
        Version = version;
    }
}