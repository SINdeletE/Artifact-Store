using ArtifactStore.Application.Interfaces.Services.Store;
using ArtifactStore.Application.Models.Requires.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseStoreItemRequire = ArtifactStore.Application.Models.Requires.Store.Purchase.PurchaseStoreItemRequire;
using SellStoreItemRequire = ArtifactStore.Application.Models.Requires.Store.Sell.SellStoreItemRequire;

namespace ArtifactStore.WebAPI.Controllers;

[ApiController]
[Route("api/store")]
[Authorize]
public class StoreController : ControllerBase
{
    private IStoreService _storeService;

    public StoreController(IStoreService storeService)
    {
        _storeService = storeService;
    }

    [HttpPost]
    [Authorize(Roles = "admin,moderator")]
    public async Task<IActionResult> AddStoreItem(AddStoreItemRequire req)
    {
        var result = await _storeService.AddStoreItem(req);
        return Created(string.Empty, result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "admin,moderator")]
    public async Task<IActionResult> DeleteStoreItem(Guid id)
    {
        await _storeService.DeleteStoreItem(
            new DeleteStoreItemRequire{Id = id});
        return Ok();
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "admin,moderator")]
    public async Task<IActionResult> UpdateStoreItem(Guid id, UpdateStoreItemRequire req)
    {
        req.Id = id;
        await _storeService.UpdateStoreItem(req);
        return Ok();
    }

    [HttpGet("page")]
    public async Task<IActionResult> GetStoreItemPage([FromQuery] GetStoreItemPageRequire req)
    {
        var result = await _storeService.GetStoreItemPage(req);
        return Ok(result);
    }

    [HttpPost("purchase")]
    public async Task<IActionResult> PurchaseStoreItem([FromQuery] PurchaseStoreItemRequire req)
    {
        await _storeService.PurchaseStoreItem(req);
        return Ok();
    }

    [HttpPost("sell")]
    public async Task<IActionResult> SellStoreItem(SellStoreItemRequire req)
    {
        await _storeService.SellStoreItem(req);
        return Ok();
    }
}
