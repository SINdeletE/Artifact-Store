namespace ArtifactStore.Auth.Application.Interfaces.Identity;

public interface IIdentityHolder
{
    public string? Role { get; set; }
    public Guid AccountId { get; set; }
}