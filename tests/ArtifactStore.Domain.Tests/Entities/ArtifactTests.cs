using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Domain.Tests.Entities;

public class ArtifactTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var artifact = new Artifact(Guid.NewGuid(), "Dragon Scale", "Scales from an ancient dragon.", null);

        Assert.Equal("Dragon Scale", artifact.Name);
        Assert.Equal("Scales from an ancient dragon.", artifact.Description);
        Assert.NotEqual(Guid.Empty, artifact.Id);
    }
}
