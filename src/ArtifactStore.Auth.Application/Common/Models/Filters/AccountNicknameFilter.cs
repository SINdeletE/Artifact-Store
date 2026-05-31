using ArtifactStore.Auth.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Auth.Domain.Entities;

namespace ArtifactStore.Auth.Application.Common.Models.Filters;

public class AccountNicknameFilter : BaseEntityFilter<Account>
{
    private readonly string _nickname;

    public AccountNicknameFilter(string nickname)
    {
        _nickname = nickname;
    }

    public override IQueryable<Account> Filter(IQueryable<Account> entities)
    {
        return entities
            .Where(b => b.Nickname == _nickname);
    }
}