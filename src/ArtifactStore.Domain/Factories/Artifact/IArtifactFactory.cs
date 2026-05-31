namespace ArtifactStore.Domain.Factories.Artifact;

public interface IArtifactFactory
{
    public Entities.Artifact Create(string name, string description, DateTimeOffset? deletedAt);
}