using ArtifactStore.Application.Interfaces.Identity;

namespace ArtifactStore.WebAPI.Identity;

public class IdentityHolder : IIdentityHolder
{
    public string? Role { get; set; }
    public Guid AccountId { get; set; }
}
