namespace ArtifactStore.Application.Models.Requires.Character;

public class AddCharacterRequire
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = "";
}