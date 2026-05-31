using ArtifactStore.Application.Interfaces.Services;
using ArtifactStore.Application.Models.Requires.Artifact;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtifactStore.WebAPI.Controllers;

[ApiController]
[Route("api/artifact")]
[Authorize(Roles = "admin")]
public class ArtifactController : ControllerBase
{
    IArtifactService _artifactService;
    
    public ArtifactController(IArtifactService artifactService)
    {
        _artifactService = artifactService;
    }
    
    [HttpPost]
    public async Task<IActionResult> AddArtifact(AddArtifactRequire req)
    {
        var result = await _artifactService.AddArtifact(req);
        return Created(string.Empty, result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateArtifact(UpdateArtifactRequire req)
    {
        await _artifactService.UpdateArtifact(req);
        return Ok();
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetArtifactById(Guid id)
    {
        var result = await _artifactService.GetArtifactById(
            new GetArtifactByIdRequire{Id = id});
        return Ok(result);
    }
}