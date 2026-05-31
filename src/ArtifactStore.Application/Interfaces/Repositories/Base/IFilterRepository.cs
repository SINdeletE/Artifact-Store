using ArtifactStore.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Domain.Common;

namespace ArtifactStore.Application.Interfaces.Repositories.Base;

public interface IFilterRepository<TEntity> where TEntity : IEntity
{
    public Task<IEnumerable<TEntity>> FindAsync(BaseEntityFilter<TEntity> filter);
}