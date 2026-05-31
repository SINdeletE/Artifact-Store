namespace ArtifactStore.Application.Models.Requires.Artifact;

public class UpdateArtifactRequire
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public uint Version { get; set; }
}