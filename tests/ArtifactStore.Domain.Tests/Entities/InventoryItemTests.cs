using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Domain.Tests.Entities;

public class InventoryItemTests
{
    private static Character CreateCharacter() =>
        new(Guid.NewGuid(), Guid.NewGuid(), "Hero", DateOnly.FromDateTime(DateTime.Today), null);

    private static Artifact CreateArtifact() => new(Guid.NewGuid(), "Dagger", "A swift dagger.", null);

    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var character = CreateCharacter();
        var artifact = CreateArtifact();

        var item = new InventoryItem(Guid.NewGuid(), character, artifact);

        Assert.Equal(character, item.Character);
        Assert.Equal(artifact, item.Artifact);
        Assert.NotEqual(Guid.Empty, item.Id);
    }
}
