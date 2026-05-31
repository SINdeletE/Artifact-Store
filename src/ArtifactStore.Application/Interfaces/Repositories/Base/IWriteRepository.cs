using ArtifactStore.Domain.Common;

namespace ArtifactStore.Application.Interfaces.Repositories.Base;

public interface IWriteRepository<TEntity> where TEntity : IEntity
{
    public Task<TEntity> InsertAsync(TEntity entity);
    public Task UpdateAsync(TEntity entity);
    public Task DeleteAsync(TEntity entity);
}