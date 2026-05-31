namespace ArtifactStore.WebApp.Responses;

public class CharacterResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = "";
    public DateOnly CreationDate { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public uint Version { get; set; }
}
