using ArtifactStore.Auth.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Auth.Domain.Entities;

namespace ArtifactStore.Auth.Application.Common.Models.Filters;

public class AccountIdFilter : BaseEntityFilter<Account>
{
    private readonly Guid _accountId;

    public AccountIdFilter(Guid id)
    {
        _accountId = id;
    }

    public override IQueryable<Account> Filter(IQueryable<Account> entities)
    {
        return entities
            .Where(b => b.Id == _accountId);
    }
}