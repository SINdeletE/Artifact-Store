using ArtifactStore.Auth.Domain.Common;

namespace ArtifactStore.Auth.Domain.Entities;

public class AccountRole : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}