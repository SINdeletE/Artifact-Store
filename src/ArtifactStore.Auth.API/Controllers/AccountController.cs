using ArtifactStore.Auth.Application.Interfaces.Services;
using ArtifactStore.Auth.Application.Models.Requires.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtifactStore.Auth.API.Controllers;

[ApiController]
[Route("api/account")]
[Authorize(Roles = "admin")]
public class AccountController : ControllerBase
{
    IAccountService _accountService;
    
    public AccountController(IAccountService accountService)
    {
        _accountService =  accountService;
    }
    
    [HttpPost]
    public async Task<IActionResult> AdminAddAccount([FromBody] AddAccountRequire req)
    {
        await _accountService.AddAccount(req);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAccountPage([FromQuery] GetAccountPageRequire req)
    {
        var accounts = await _accountService.GetAccountPage(req);
        return Ok(accounts);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> AdminDeleteAccount(Guid id)
    {
        await _accountService.DeleteAccount(new DeleteAccountRequire { Id = id });
        return Ok();
    }
}