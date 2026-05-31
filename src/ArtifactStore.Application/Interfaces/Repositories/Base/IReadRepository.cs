using ArtifactStore.Domain.Common;

namespace ArtifactStore.Application.Interfaces.Repositories.Base;

public interface IReadRepository<TEntity> where TEntity : IEntity
{
    public Task<TEntity> GetByIdAsync(Guid id);
}