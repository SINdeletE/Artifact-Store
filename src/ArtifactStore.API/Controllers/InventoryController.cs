using ArtifactStore.Application.Interfaces.Identity;
using ArtifactStore.Application.Interfaces.Services;
using ArtifactStore.Application.Models.Requires.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtifactStore.WebAPI.Controllers;

[ApiController]
[Route("api/inventory")]
[Authorize]
public class InventoryController : ControllerBase
{
    IInventoryService _inventoryService;
    IIdentityHolder _identityHolder;
    
    public InventoryController(IInventoryService inventoryService, IIdentityHolder identityHolder)
    {
        _inventoryService = inventoryService;
        _identityHolder = identityHolder;
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteInventoryItem(Guid id)
    {
        await _inventoryService.DeleteInventoryItem(
            new DeleteInventoryItemRequire
            {
                Id = id,
                AccountId = _identityHolder.AccountId,
                IsPrivileged = _identityHolder.Role is "admin" or "moderator"
            });
        return Ok();
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetInventoryItemById(Guid id)
    {
        var inventoryItem = await _inventoryService.GetInventoryItemById(
            new GetInventoryItemById{Id = id});
        return Ok(inventoryItem);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetInventoryPageByCharacterId([FromQuery] GetInventoryPageByCharacterIdRequire req)
    {
        var page = await _inventoryService.GetInventoryPageByCharacterId(req);
        return Ok(page);
    }

    [HttpPost]
    [Authorize(Roles = "admin,moderator")]
    public async Task<IActionResult> AddInventoryItem(AddInventoryItemRequire req)
    {
        var result = await _inventoryService.AddInventoryItem(req);
        return Created(string.Empty, result);
    }
}