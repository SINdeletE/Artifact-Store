using ArtifactStore.Auth.Application.Interfaces.Identity;

namespace ArtifactStore.Auth.API.Identity;

public class IdentityHolder : IIdentityHolder
{
    public string? Role { get; set; }
    public Guid AccountId { get; set; }
}
