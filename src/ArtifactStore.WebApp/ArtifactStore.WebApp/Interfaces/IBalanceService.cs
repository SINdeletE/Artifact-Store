using ArtifactStore.WebApp.Requires.Balance;
using ArtifactStore.WebApp.Responses;

namespace ArtifactStore.WebApp.Interfaces;

public interface IBalanceService
{
    public Task<BalanceResponse> GetBalanceById(Guid id);
    public Task UpdateBalance(BalanceUpdateRequire req);
    public Task<BalanceResponse> AddBalanceWithCurrency(BalanceAddWithCurrencyRequire req);
    public Task<IReadOnlyList<BalanceResponse>> GetCharacterBalances(CharacterBalancesRequire req);
}
