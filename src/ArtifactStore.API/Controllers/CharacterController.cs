using ArtifactStore.Application.Interfaces.Identity;
using ArtifactStore.Application.Interfaces.Services;
using ArtifactStore.Application.Models.Requires.Character;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtifactStore.WebAPI.Controllers;

[ApiController]
[Route("api/character")]
[Authorize]
public class CharacterController : ControllerBase
{
    ICharacterService _characterService;
    IIdentityHolder _identityHolder;
    
    public CharacterController(ICharacterService characterService, IIdentityHolder identityHolder)
    {
        _characterService = characterService;
        _identityHolder = identityHolder;
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "admin,moderator")]
    public async Task<IActionResult> GetCharacterById(Guid id)
    {
        var result = await _characterService.GetCharacterById(
            new GetCharacterByIdRequire{Id = id});
        return Ok(result);
    }

    [HttpGet("page")]
    [Authorize(Roles = "admin,moderator")]
    public async Task<IActionResult> GetCharacterPage([FromQuery] GetCharacterPageRequire req)
    {
        var result = await _characterService.GetCharacterPage(req);
        return Ok(result);
    }
    
    [HttpGet("my/page")]
    public async Task<IActionResult> GetUserCharacterPage([FromQuery] GetCharacterPageRequire req)
    {
        var result = await _characterService.GetUserCharacterPage(
            new GetUserCharacterPageRequire
            {
                AccountId = _identityHolder.AccountId,
                Page = req.Page,
                PageSize = req.PageSize
            });
        return Ok(result);
    }
    
    [HttpGet("my/all")]
    public async Task<IActionResult> GetUserCharacters()
    {
        var result = await _characterService.GetUserCharacters(
            _identityHolder.AccountId);
        
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCharacter(Guid id)
    {
        await _characterService.DeleteCharacter(
            new DeleteCharacterRequire
            {
                Id = id,
                AccountId = _identityHolder.AccountId,
                IsPrivileged = _identityHolder.Role is "admin" or "moderator"
            });
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> AddCharacter([FromBody] AddCharacterCommonRequire req)
    {
        var result = await _characterService.AddCharacter(
            new AddCharacterRequire
            {
                UserId = _identityHolder.AccountId,
                Name = req.Name
            });
        return Created(string.Empty, result);
    }
}