using ArtifactStore.WebApp.Error;
using ArtifactStore.WebApp.HandlerAdders;
using ArtifactStore.WebApp.Interfaces;
using ArtifactStore.WebApp.Requires.Balance;
using ArtifactStore.WebApp.Responses;
using ArtifactStore.WebApp.Shared.Http;
using ArtifactStore.WebApp.Shared.Routes;

namespace ArtifactStore.WebApp.Services;

public class BalanceService : IBalanceService
{
    IErrorHandler _errorHandler;
    HttpClient _httpClient;
    IHttpContextAccessor _httpContextAccessor;

    public BalanceService(IHttpClientFactory factory, IErrorHandler errorHandler, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = factory.CreateClient(HttpClientName.Main);
        _errorHandler = errorHandler;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<BalanceResponse> GetBalanceById(Guid id)
    {
        var response = await _httpClient.GetAsync(BalanceRoute.GetByIdUrl(id));
        await _errorHandler.EnsureSuccessOrThrow(response);

        var result = await response.Content.ReadFromJsonAsync<BalanceResponse>();
        return result!;
    }

    public async Task UpdateBalance(BalanceUpdateRequire req)
    {
        var response = await _httpClient.PutAsJsonAsync(BalanceRoute.UpdateUrl(req.Id), new
        {
            id = req.Id,
            character = req.Character,
            currency = req.Currency,
            amount = req.Amount,
            version = req.Version
        });
        await _errorHandler.EnsureSuccessOrThrow(response);
    }

    public async Task<BalanceResponse> AddBalanceWithCurrency(BalanceAddWithCurrencyRequire req)
    {
        var response = await _httpClient.PostAsJsonAsync(BalanceRoute.AddWithCurrency, new
        {
            characterId = req.CharacterId,
            currencyId = req.CurrencyId,
            amount = req.Amount
        });
        await _errorHandler.EnsureSuccessOrThrow(response);

        var result = await response.Content.ReadFromJsonAsync<BalanceResponse>();
        return result!;
    }

    public async Task<IReadOnlyList<BalanceResponse>> GetCharacterBalances(CharacterBalancesRequire req)
    {
        TokenHandlerAdder.Add(_httpClient, _httpContextAccessor);
        var response = await _httpClient.GetAsync(BalanceRoute.GetCharacterBalancesUrl(req.CharacterId));
        await _errorHandler.EnsureSuccessOrThrow(response);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<BalanceResponse>>();
        return result?.ToArray() ?? [];
    }
}
