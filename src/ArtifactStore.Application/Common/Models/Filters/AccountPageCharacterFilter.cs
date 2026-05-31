using ArtifactStore.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Domain.Common;
using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Common.Models.Filters;

public class AccountPageCharacterFilter : BaseEntityFilter<Character>
{
    private readonly Guid _accountId;
    private readonly int _page;
    private readonly int _pageSize;

    public AccountPageCharacterFilter(Guid AccountId, int page, int pageSize)
    {
        _accountId = AccountId;
        _page = page;
        _pageSize = pageSize;
    }

    public override IQueryable<Character> Filter(IQueryable<Character> entities)
    {
        return entities
            .Where(c => c.Id == _accountId)
            .Skip((_page - 1) * _pageSize)
            .Take(_pageSize);
    }
}