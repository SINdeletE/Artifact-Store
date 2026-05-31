using ArtifactStore.Application.Models.Requires.Artifact;
using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Interfaces.Services;

public interface IArtifactService
{
    public Task<Artifact> AddArtifact(AddArtifactRequire req);
    public Task<Artifact> GetArtifactById(GetArtifactByIdRequire req);
    public Task UpdateArtifact(UpdateArtifactRequire req);
}