using ArtifactStore.Application.Interfaces.Services;
using ArtifactStore.Application.Models.Requires.StoreTransaction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtifactStore.WebAPI.Controllers;

[ApiController]
[Route("api/storetransaction")]
[Authorize(Roles = "admin,moderator")]
public class StoreTransactionController : ControllerBase
{
    IStoreTransactionService _storeTransactionService;

    public StoreTransactionController(IStoreTransactionService storeTransactionService)
    {
        _storeTransactionService = storeTransactionService;
    }

    [HttpGet("page")]
    public async Task<IActionResult> GetStoreTransactionPage([FromQuery] GetStoreTransactionPageRequire req)
    {
        var page = await _storeTransactionService.GetStoreTransactionPage(req);
        return Ok(new {page});
    }
}