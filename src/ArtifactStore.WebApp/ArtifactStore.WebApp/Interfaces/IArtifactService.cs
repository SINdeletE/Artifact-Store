using ArtifactStore.WebApp.Requires.Artifact;
using ArtifactStore.WebApp.Responses;

namespace ArtifactStore.WebApp.Interfaces;

public interface IArtifactService
{
    public Task<ArtifactResponse> AddArtifact(ArtifactAddRequire req);
    public Task UpdateArtifact(ArtifactUpdateRequire req);
    public Task<ArtifactResponse> GetArtifactById(Guid id);
}
