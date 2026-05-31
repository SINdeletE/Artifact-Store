using ArtifactStore.Auth.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Auth.Domain.Entities;

namespace ArtifactStore.Auth.Application.Common.Models.Filters;

public class AccountRoleFilter : BaseEntityFilter<AccountRole>
{
    private readonly string _name;

    public AccountRoleFilter(string name)
    {
        _name = name;
    }

    public override IQueryable<AccountRole> Filter(IQueryable<AccountRole> entities)
    {
        return entities
            .Where(b => b.Name == _name);
    }
}