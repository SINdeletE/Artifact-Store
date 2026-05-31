namespace ArtifactStore.Application.Models.Requires.Character;

public class DeleteCharacterRequire
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public bool IsPrivileged { get; set; } = false;
}