using ArtifactStore.Application.Interfaces.Repositories.Base;
using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Interfaces.Repositories;

public interface IArtifactRepository : IReadRepository<Artifact>, IWriteRepository<Artifact>
{
    // public Task AddArtifact(Artifact artifact);
    // public Task<Artifact?> GetArtifactById(Guid id);
    // public Task<bool> UpdateArtifact(Artifact artifact);
}