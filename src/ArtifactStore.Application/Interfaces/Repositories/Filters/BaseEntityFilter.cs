using ArtifactStore.Domain.Common;

namespace ArtifactStore.Application.Interfaces.Repositories.Filters;

public abstract class BaseEntityFilter<T> where T : IEntity
{
    public virtual IQueryable<T> Filter(IQueryable<T> entities) => entities;
}