namespace ArtifactStore.WebApp.Responses;

public class ArtifactResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTimeOffset? DeletedAt { get; set; }
    public uint Version { get; set; }
}
