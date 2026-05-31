namespace ArtifactStore.Application.Models.Requires.Character;

public class GetUserCharacterPageRequire
{
    public Guid AccountId { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}