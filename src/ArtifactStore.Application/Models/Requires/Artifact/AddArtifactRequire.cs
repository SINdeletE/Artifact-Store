namespace ArtifactStore.Application.Models.Requires.Artifact;

public class AddArtifactRequire
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";

    public AddArtifactRequire(string name, string description)
    {
        Name = name;
        Description = description;
    }
}