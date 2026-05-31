namespace ArtifactStore.WebApp.Requires.Artifact;

public class ArtifactUpdateRequire
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public uint Version { get; set; }
}
