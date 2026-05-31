using ArtifactStore.Domain.Factories.Artifact;

namespace ArtifactStore.Domain.Tests.Factories;

public class ArtifactFactoryTests
{
    private readonly ArtifactFactory _factory = new();

    [Fact]
    public void Create_SetsPropertiesCorrectly()
    {
        var artifact = _factory.Create("Sword of Dawn", "A legendary blade.", null);

        Assert.Equal("Sword of Dawn", artifact.Name);
        Assert.Equal("A legendary blade.", artifact.Description);
        Assert.NotEqual(Guid.Empty, artifact.Id);
    }


}
