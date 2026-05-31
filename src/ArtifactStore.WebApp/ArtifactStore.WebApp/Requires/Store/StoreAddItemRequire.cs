using ArtifactStore.WebApp.Responses;

namespace ArtifactStore.WebApp.Requires.Store;

public class StoreAddItemRequire
{
    public required CurrencyResponse Currency { get; set; }
    public required ArtifactResponse Artifact { get; set; }
    public decimal Price { get; set; }
}
