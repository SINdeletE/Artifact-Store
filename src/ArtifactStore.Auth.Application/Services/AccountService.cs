using ArtifactStore.Auth.Application.Common.Models;
using ArtifactStore.Auth.Application.Common.Models.Filters;
using ArtifactStore.Auth.Application.Interfaces.Crypt;
using ArtifactStore.Auth.Application.Interfaces.Repositories.Account;
using ArtifactStore.Auth.Application.Interfaces.Services;
using ArtifactStore.Auth.Application.Models.Requires.Account;
using ArtifactStore.Auth.Domain.Entities;
using ArtifactStore.Auth.Domain.Exceptions;
using ArtifactStore.Auth.Domain.Factories.Account;
using Microsoft.Extensions.Logging;

namespace ArtifactStore.Auth.Application.Services;

public class AccountService : IAccountService
{
    ILogger<AccountService> _logger;
    
    IAccountRepository _accountRepository;
    IAccountRoleRepository _accountRoleRepository;
    IAccountFactory _accountFactory;
    
    IAccountPasswordCryptor _accountPasswordCryptor;

    public AccountService(IAccountRepository accountRepository, IAccountRoleRepository accountRoleRepository,
        IAccountFactory accountFactory,
        IAccountPasswordCryptor accountPasswordCryptor,
        ILogger<AccountService> logger)
    {
        _accountRepository = accountRepository;
        _accountRoleRepository = accountRoleRepository;
        
        _accountFactory = accountFactory;
        _accountPasswordCryptor = accountPasswordCryptor;
        
        _logger = logger;
    }
    
    public async Task<Account> AddAccount(AddAccountRequire req)
    {
        try
        {
            var accountRoleEnumerable = await _accountRoleRepository.FindAsync(new AccountRoleFilter(req.Role));
            var accountRole = accountRoleEnumerable.First();
            _logger.LogInformation("Fetched account role {Role}", accountRole.Name);
            
            var account = _accountFactory.Create(accountRole, req.Nickname, _accountPasswordCryptor.Crypt(req.Password), req.Email, null);
            _logger.LogInformation("Created account {AccountId}", account.Id);
            
            await _accountRepository.InsertAsync(account);
            _logger.LogInformation("Inserted account {AccountId}", account.Id);
            
            return account;
        }
        catch (InvalidOperationException ex)
        {
            throw new UnauthorizedException("Invalid role");
        }
    }

    public async Task DeleteAccount(DeleteAccountRequire req)
    {
        var inventoryItem = await _accountRepository.GetByIdAsync(req.Id);
        _logger.LogInformation("Fetched account {AccountId}", req.Id);
        
        await _accountRepository.DeleteAsync(inventoryItem);
        _logger.LogInformation("Deleted account {AccountId}", req.Id);
    }

    public async Task UpdateAccount(UpdateAccountRequire req)
    {
        var account = await _accountRepository.GetByIdAsync(req.Id);
        _logger.LogInformation("Fetched account {AccountId}", req.Id);
        
        account.Nickname = req.Nickname;
        account.Password = _accountPasswordCryptor.Crypt(req.Password);
        account.Email = req.Email;
        account.RegisterDate = req.RegisterDate;
        
        await _accountRepository.UpdateAsync(account);
        _logger.LogInformation("Updated account {AccountId}", req.Id);
    }

    public async Task<Account> GetAccountById(GetAccountByIdRequire req)
    {
        var result = await _accountRepository.GetByIdAsync(req.Id);
        _logger.LogInformation("Fetched account {AccountId}", req.Id);
        
        return result;
    }

    public async Task<PageResult<Account>> GetAccountPage(GetAccountPageRequire req)
    {
        var accountPage = await _accountRepository.FindAsync(new PageEntityFilter<Account>(req.Page, req.PageSize));
        _logger.LogInformation("Fetched account page");
        
        return new PageResult<Account>(accountPage, accountPage.Count());
    }

    public async Task<Account> GetAccountByNickname(GetAccountByNicknameRequire req)
    {
        try
        {
            var accountEnumerable = await _accountRepository.FindAsync(new AccountNicknameFilter(req.Nickname));
            var account = accountEnumerable.First();
            
            _logger.LogInformation("Fetched account {AccountId}", account.Id);
            
            return account;
        }
        catch (InvalidOperationException ex)
        {
            throw new UnauthorizedException("Invalid username or password");
        }
    }
}