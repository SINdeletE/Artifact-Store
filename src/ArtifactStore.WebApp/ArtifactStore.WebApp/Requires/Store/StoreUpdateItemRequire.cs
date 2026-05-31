using ArtifactStore.WebApp.Responses;

namespace ArtifactStore.WebApp.Requires.Store;

public class StoreUpdateItemRequire
{
    public Guid Id { get; set; }
    public required CurrencyResponse Currency { get; set; }
    public required ArtifactResponse Artifact { get; set; }
    public decimal Price { get; set; }
    public uint Version { get; set; }
}
