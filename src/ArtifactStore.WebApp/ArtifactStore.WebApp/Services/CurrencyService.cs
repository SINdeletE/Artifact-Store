using ArtifactStore.WebApp.Error;
using ArtifactStore.WebApp.Interfaces;
using ArtifactStore.WebApp.Responses;
using ArtifactStore.WebApp.Shared.Http;
using ArtifactStore.WebApp.Shared.Routes;

namespace ArtifactStore.WebApp.Services;

public class CurrencyService : ICurrencyService
{
    IErrorHandler _errorHandler;
    HttpClient _httpClient;

    public CurrencyService(IHttpClientFactory factory, IErrorHandler errorHandler)
    {
        _httpClient = factory.CreateClient(HttpClientName.Main);
        _errorHandler = errorHandler;
    }

    public async Task<CurrencyResponse> GetCurrencyById(Guid id)
    {
        var response = await _httpClient.GetAsync(CurrencyRoute.GetByIdUrl(id));
        await _errorHandler.EnsureSuccessOrThrow(response);

        var result = await response.Content.ReadFromJsonAsync<CurrencyResponse>();
        return result!;
    }
}
