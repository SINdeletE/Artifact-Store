using ArtifactStore.Auth.Domain.Entities;

namespace ArtifactStore.Auth.Domain.Factories.Account;

public class AccountFactory : IAccountFactory
{
    public Auth.Domain.Entities.Account Create(AccountRole accountRole, string nickname, string password, string email, DateTimeOffset? deletedAt)
    { 
        var registerDate = DateOnly.FromDateTime(DateTime.UtcNow);
        return new Auth.Domain.Entities.Account(Guid.NewGuid(), accountRole, nickname, password, email, registerDate, deletedAt);
    }
}