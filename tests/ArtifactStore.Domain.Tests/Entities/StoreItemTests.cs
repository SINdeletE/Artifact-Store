using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Domain.Tests.Entities;

public class StoreItemTests
{
    private static Currency CreateCurrency() => new(Guid.NewGuid(), "Gold", null);
    private static Artifact CreateArtifact() => new(Guid.NewGuid(), "Sword", "Sharp blade.", null);

    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var currency = CreateCurrency();
        var artifact = CreateArtifact();
        var price = 99.99m;

        var item = new StoreItem(Guid.NewGuid(), currency, artifact, price);

        Assert.Equal(currency, item.Currency);
        Assert.Equal(artifact, item.Artifact);
        Assert.Equal(price, item.Price);
        Assert.NotEqual(Guid.Empty, item.Id);
    }

    [Fact]
    public void Price_CanBeUpdated()
    {
        var item = new StoreItem(Guid.NewGuid(), CreateCurrency(), CreateArtifact(), 50m);

        item.Price = 75m;

        Assert.Equal(75m, item.Price);
    }
}
