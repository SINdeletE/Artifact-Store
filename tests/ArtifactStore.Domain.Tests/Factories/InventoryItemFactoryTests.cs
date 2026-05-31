using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Factories.Inventory;

namespace ArtifactStore.Domain.Tests.Factories;

public class InventoryItemFactoryTests
{
    private readonly InventoryItemFactory _factory = new();

    [Fact]
    public void Create_SetsPropertiesCorrectly()
    {
        var character = new Character(Guid.NewGuid(), Guid.NewGuid(), "Hero", DateOnly.FromDateTime(DateTime.Today), null);
        var artifact = new Artifact(Guid.NewGuid(), "Dagger", "A swift dagger.", null);

        var item = _factory.Create(character, artifact);

        Assert.Equal(character, item.Character);
        Assert.Equal(artifact, item.Artifact);
        Assert.NotEqual(Guid.Empty, item.Id);
    }
}
