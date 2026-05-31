using ArtifactStore.Auth.Application.Common.Models;
using ArtifactStore.Auth.Application.Models.Requires.Account;
using ArtifactStore.Auth.Domain.Entities;

namespace ArtifactStore.Auth.Application.Interfaces.Services;

public interface IAccountService
{
    public Task<Account> AddAccount(AddAccountRequire req);
    public Task UpdateAccount(UpdateAccountRequire req);
    public Task DeleteAccount(DeleteAccountRequire req);
    public Task<Account> GetAccountById(GetAccountByIdRequire req);
    public Task<PageResult<Account>> GetAccountPage(GetAccountPageRequire req);
    public Task<Account> GetAccountByNickname(GetAccountByNicknameRequire req);
}