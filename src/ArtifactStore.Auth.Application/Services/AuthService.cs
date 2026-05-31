using ArtifactStore.Auth.Application.Interfaces.Crypt;
using ArtifactStore.Auth.Application.Interfaces.Services;
using ArtifactStore.Auth.Application.Models.Requires.Account;
using ArtifactStore.Auth.Application.Models.Requires.Auth;
using ArtifactStore.Auth.Application.Models.Requires.Token;
using ArtifactStore.Auth.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace ArtifactStore.Auth.Application.Services;

public class AuthService : IAuthService
{
    ILogger<AuthService> _logger;
    
    IAccountService _accountService;
    ITokenService _tokenService;
    IAccountPasswordCryptor _accountPasswordCryptor;

    public AuthService(IAccountService accountService, ITokenService tokenService, 
        IAccountPasswordCryptor accountPasswordCryptor, ILogger<AuthService> logger)
    {
        _accountService = accountService;
        _tokenService = tokenService;
        _accountPasswordCryptor = accountPasswordCryptor;
        
        _logger = logger;
    }

    public async Task<string> LoginAsync(AuthLoginRequire req)
    {
        var account = await _accountService.GetAccountByNickname(new GetAccountByNicknameRequire
        {
            Nickname = req.Nickname
        });

        if (!_accountPasswordCryptor.Compare(
                req.Password,
                account.Password))
        {
            throw new UnauthorizedException("Invalid username or password");
        }

        _logger.LogInformation("Logged account {AccountId}", account.Id);
        var token = _tokenService.GenerateToken(new GenerateTokenRequire(account));
        _logger.LogInformation("Generated token for account {AccountId}", account.Id);
        
        return token;
    }

    public async Task RegisterAsync(AuthRegisterRequire req)
    {
        var account = await _accountService.AddAccount(new AddAccountRequire
        {
            Role = "default_user",
            Nickname = req.Nickname,
            Password = req.Password,
            Email = req.Email
        });
        
        _logger.LogInformation("Registrated account {AccountId}", account.Id);
    }
}
