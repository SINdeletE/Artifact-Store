namespace ArtifactStore.WebApp.Shared.Models.Artifact;

public sealed class Artifact
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTimeOffset? DeletedAt { get; set; }
    public uint Version { get; set; }
}
