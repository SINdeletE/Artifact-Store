using ArtifactStore.WebApp.Error;
using ArtifactStore.WebApp.Interfaces;
using ArtifactStore.WebApp.Requires.StoreTransaction;
using ArtifactStore.WebApp.Responses;
using ArtifactStore.WebApp.Shared.Http;
using ArtifactStore.WebApp.Shared.Routes;

namespace ArtifactStore.WebApp.Services;

public class StoreTransactionService : IStoreTransactionService
{
    IErrorHandler _errorHandler;
    HttpClient _httpClient;

    public StoreTransactionService(IHttpClientFactory factory, IErrorHandler errorHandler)
    {
        _httpClient = factory.CreateClient(HttpClientName.Main);
        _errorHandler = errorHandler;
    }

    public async Task<PageResponse<StoreTransactionResponse>> GetStoreTransactionPage(StoreTransactionGetPageRequire req)
    {
        var response = await _httpClient.GetAsync(ServiceUrl.WithPage(StoreTransactionRoute.GetPage, req.Page, req.PageSize));
        await _errorHandler.EnsureSuccessOrThrow(response);

        var result = await response.Content.ReadFromJsonAsync<StoreTransactionPageResponse>();
        return result!.Page!;
    }
}
