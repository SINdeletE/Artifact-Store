using ArtifactStore.Auth.Domain.Common;

namespace ArtifactStore.Auth.Application.Interfaces.Repositories.Filters;

public abstract class BaseEntityFilter<T> where T : IEntity
{
    public virtual IQueryable<T> Filter(IQueryable<T> entities) => entities;
}