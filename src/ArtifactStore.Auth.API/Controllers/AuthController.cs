using ArtifactStore.Auth.Application.Interfaces.Services;
using ArtifactStore.Auth.Application.Models.Requires.Auth;
using Microsoft.AspNetCore.Mvc;

namespace ArtifactStore.Auth.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    IAuthService _authService;
    
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthLoginRequire req)
    {
        var token = await _authService.LoginAsync(req);
        return Ok(new { token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthRegisterRequire req)
    {
        await _authService.RegisterAsync(req);
        return Ok();
    }
}