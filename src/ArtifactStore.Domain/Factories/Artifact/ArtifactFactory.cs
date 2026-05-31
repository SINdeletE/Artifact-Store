namespace ArtifactStore.Domain.Factories.Artifact;

public class ArtifactFactory : IArtifactFactory
{
    public Entities.Artifact Create(string name, string description, DateTimeOffset? deletedAt)
    {
        return new Entities.Artifact(Guid.NewGuid(), name, description, deletedAt);
    }
}