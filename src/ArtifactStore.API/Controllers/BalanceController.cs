using ArtifactStore.Application.Interfaces.Identity;
using ArtifactStore.Application.Interfaces.Services;
using ArtifactStore.Application.Models.Requires.Balance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtifactStore.WebAPI.Controllers;

[ApiController]
[Route("api/balance")]
[Authorize]
public class BalanceController : ControllerBase
{
    IBalanceService _balanceService;
    IIdentityHolder _identityHolder;
    
    public BalanceController(IBalanceService balanceService, IIdentityHolder identityHolder)
    {
        _balanceService = balanceService;
        _identityHolder = identityHolder;
    }
    
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "admin,moderator")]
    public async Task<IActionResult> GetBalanceById(Guid id)
    {
        var result = await _balanceService.GetBalanceById(
            new GetBalanceByIdRequire{Id = id});
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "admin,moderator")]
    public async Task<IActionResult> UpdateBalance(Guid id, UpdateBalanceRequire req)
    {
        req.Id = id;
        await _balanceService.UpdateBalance(req);
        return Ok();
    }

    [HttpPost]
    [Authorize(Roles = "default_user")]
    public async Task<IActionResult> AddBalanceWithCurrency([FromBody] AddBalanceWithCurrencyCommonRequire req)
    {
        var result = await _balanceService.AddBalanceWithCurrency(
            new AddBalanceWithCurrencyRequire
            {
                CharacterId = req.CharacterId,
                CurrencyId = req.CurrencyId,
                Amount = req.Amount,
                AccountId = _identityHolder.AccountId,
                IsPrivileged = _identityHolder.Role is "admin" or "moderator"
            });
        return Created(string.Empty, result);
    }
    
    [HttpGet("character")]
    public async Task<IActionResult> GetBalancesByCharacterId([FromQuery] Guid id)
    {
        var result = await _balanceService.GetCharacterBalances(id);
        return Ok(result);
    }
}