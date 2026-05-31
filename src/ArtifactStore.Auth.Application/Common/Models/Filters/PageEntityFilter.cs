using ArtifactStore.Auth.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Auth.Domain.Common;

namespace ArtifactStore.Auth.Application.Common.Models.Filters;

public class PageEntityFilter<TEntity> : BaseEntityFilter<TEntity> where TEntity : IEntity
{
    private readonly int _page;
    private readonly int _pageSize;

    public PageEntityFilter(int page, int pageSize)
    {
        _page = page;
        _pageSize = pageSize;
    }

    public override IQueryable<TEntity> Filter(IQueryable<TEntity> entities)
    {
        return entities
            .Skip((_page - 1) * _pageSize)
            .Take(_pageSize);
    }
}