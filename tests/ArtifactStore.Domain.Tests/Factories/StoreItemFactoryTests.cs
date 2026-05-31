using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Factories.Store;

namespace ArtifactStore.Domain.Tests.Factories;

public class StoreItemFactoryTests
{
    private readonly StoreItemFactory _factory = new();

    [Fact]
    public void Create_SetsPropertiesCorrectly()
    {
        var currency = new Currency(Guid.NewGuid(), "Gold", null);
        var artifact = new Artifact(Guid.NewGuid(), "Sword", "Sharp blade.", null);

        var item = _factory.Create(currency, artifact, 49.99m);

        Assert.Equal(currency, item.Currency);
        Assert.Equal(artifact, item.Artifact);
        Assert.Equal(49.99m, item.Price);
        Assert.NotEqual(Guid.Empty, item.Id);
    }
}
