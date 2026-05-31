using ArtifactStore.Application.Models.Requires.Balance;
using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Interfaces.Services;

public interface IBalanceService
{
    public Task<Balance> GetBalanceById(GetBalanceByIdRequire req);
    public Task UpdateBalance(UpdateBalanceRequire req);
    public Task<Balance> AddBalanceWithCurrency(AddBalanceWithCurrencyRequire req);
    public Task<Balance> GetBalanceWithCharacterCurrency(GetBalanceWithCharacterCurrencyRequire req);
    public Task<IEnumerable<Balance>> GetCharacterBalances(Guid characterId);
}