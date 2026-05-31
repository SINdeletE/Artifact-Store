using ArtifactStore.WebApp.Error;
using ArtifactStore.WebApp.HandlerAdders;
using ArtifactStore.WebApp.Interfaces;
using ArtifactStore.WebApp.Requires.Store;
using ArtifactStore.WebApp.Responses;
using ArtifactStore.WebApp.Shared.Http;
using ArtifactStore.WebApp.Shared.Routes;

namespace ArtifactStore.WebApp.Services;

public class StoreService : IStoreService
{
    IErrorHandler _errorHandler;
    HttpClient _httpClient;
    IHttpContextAccessor _httpContextAccessor;

    public StoreService(IHttpClientFactory factory, IErrorHandler errorHandler, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = factory.CreateClient(HttpClientName.Main);
        _errorHandler = errorHandler;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<StoreItemResponse> AddStoreItem(StoreAddItemRequire req)
    {
        var response = await _httpClient.PostAsJsonAsync(StoreRoute.Add, new
        {
            currency = req.Currency,
            artifact = req.Artifact,
            price = req.Price
        });
        await _errorHandler.EnsureSuccessOrThrow(response);

        var result = await response.Content.ReadFromJsonAsync<StoreItemResponse>();
        return result!;
    }

    public async Task DeleteStoreItem(Guid id)
    {
        var response = await _httpClient.DeleteAsync(StoreRoute.DeleteUrl(id));
        await _errorHandler.EnsureSuccessOrThrow(response);
    }

    public async Task UpdateStoreItem(StoreUpdateItemRequire req)
    {
        var response = await _httpClient.PutAsJsonAsync(StoreRoute.UpdateUrl(req.Id), new
        {
            id = req.Id,
            currency = req.Currency,
            artifact = req.Artifact,
            price = req.Price,
            version = req.Version
        });
        await _errorHandler.EnsureSuccessOrThrow(response);
    }

    public async Task<PageResponse<StoreItemResponse>> GetStoreItemPage(StoreGetItemPageRequire req)
    {
        TokenHandlerAdder.Add(_httpClient, _httpContextAccessor);
        var response = await _httpClient.GetAsync(ServiceUrl.WithPage(StoreRoute.GetPage, req.Page, req.PageSize));
        await _errorHandler.EnsureSuccessOrThrow(response);

        var result = await response.Content.ReadFromJsonAsync<PageResponse<StoreItemResponse>>();
        return result!;
    }

    public async Task PurchaseStoreItem(StorePurchaseItemRequire req)
    {
        TokenHandlerAdder.Add(_httpClient, _httpContextAccessor);
        var response = await _httpClient.PostAsync(StoreRoute.PurchaseUrl(req.StoreItemId, req.CharacterId), null);
        await _errorHandler.EnsureSuccessOrThrow(response);
    }

    public async Task SellStoreItem(StoreSellItemRequire req)
    {
        var response = await _httpClient.PostAsJsonAsync(StoreRoute.Sell, new
        {
            inventoryItemId = req.InventoryItemId,
            currencyId = req.CurrencyId,
            price = req.Price
        });
        await _errorHandler.EnsureSuccessOrThrow(response);
    }
}
