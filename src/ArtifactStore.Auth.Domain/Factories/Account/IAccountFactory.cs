using ArtifactStore.Auth.Domain.Entities;

namespace ArtifactStore.Auth.Domain.Factories.Account;

public interface IAccountFactory
{
    public Auth.Domain.Entities.Account Create(AccountRole accountRole, string nickname, string password, string email, DateTimeOffset? deletedAt);
}