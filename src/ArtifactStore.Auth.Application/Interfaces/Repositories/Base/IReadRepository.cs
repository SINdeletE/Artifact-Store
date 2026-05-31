using ArtifactStore.Auth.Domain.Common;

namespace ArtifactStore.Auth.Application.Interfaces.Repositories.Base;

public interface IReadRepository<TEntity> where TEntity : IEntity
{
    public Task<TEntity> GetByIdAsync(Guid id);
}