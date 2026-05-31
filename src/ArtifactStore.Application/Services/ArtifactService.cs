using ArtifactStore.Application.Interfaces.Repositories;
using ArtifactStore.Application.Models.Requires.Artifact;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Factories.Artifact;
using Microsoft.Extensions.Logging;

namespace ArtifactStore.Application.Services;

public class ArtifactService : IArtifactService
{
    ILogger<ArtifactService> _logger;
    
    private IArtifactRepository _artifactRepository;
    private IArtifactFactory _artifactFactory;

    public ArtifactService(IArtifactRepository artifactRepository, IArtifactFactory artifactFactory,
        ILogger<ArtifactService> logger)
    {
        _logger = logger;
        
        _artifactRepository = artifactRepository;
        _artifactFactory = artifactFactory;
    }
    
    public async Task<Artifact> AddArtifact(AddArtifactRequire req)
    {
        var artifact = _artifactFactory.Create(req.Name, req.Description, null);
        _logger.LogInformation("Created artifact {ArtifactId}", artifact.Id);
        
        var result = await _artifactRepository.InsertAsync(artifact);
        _logger.LogInformation("Inserted artifact {ArtifactId}", artifact.Id);
        
        return result;
    }

    public async Task<Artifact> GetArtifactById(GetArtifactByIdRequire req)
    {
        var result =  await _artifactRepository.GetByIdAsync(req.Id);
        _logger.LogInformation("Fetched artifact {ArtifactId}", result.Id);
        
        return result;
    }

    public async Task UpdateArtifact(UpdateArtifactRequire req)
    {
        var artifact = await _artifactRepository.GetByIdAsync(req.Id);
        _logger.LogInformation("Fetched artifact {ArtifactId}", artifact.Id);
        
        artifact.Name = req.Name;
        artifact.Description = req.Description;
        artifact.Version = req.Version;
        
        await _artifactRepository.UpdateAsync(artifact);
        _logger.LogInformation("Updated artifact {ArtifactId}", artifact.Id);
    }
}