using ArtifactStore.Auth.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Auth.Domain.Common;

namespace ArtifactStore.Auth.Application.Interfaces.Repositories.Base;

public interface IFilterRepository<TEntity> where TEntity : IEntity
{
    public Task<IEnumerable<TEntity>> FindAsync(BaseEntityFilter<TEntity> filter);
}