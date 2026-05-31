using ArtifactStore.WebApp.Error;
using ArtifactStore.WebApp.HandlerAdders;
using ArtifactStore.WebApp.Interfaces;
using ArtifactStore.WebApp.Requires.Inventory;
using ArtifactStore.WebApp.Responses;
using ArtifactStore.WebApp.Shared.Http;
using ArtifactStore.WebApp.Shared.Routes;

namespace ArtifactStore.WebApp.Services;

public class InventoryService : IInventoryService
{
    IErrorHandler _errorHandler;
    HttpClient _httpClient;
    IHttpContextAccessor _httpContextAccessor;

    public InventoryService(IHttpClientFactory factory, IErrorHandler errorHandler,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = factory.CreateClient(HttpClientName.Main);
        _errorHandler = errorHandler;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task DeleteInventoryItem(Guid id)
    {
        TokenHandlerAdder.Add(_httpClient, _httpContextAccessor);
        var response = await _httpClient.DeleteAsync(InventoryRoute.DeleteUrl(id));
        await _errorHandler.EnsureSuccessOrThrow(response);
    }

    public async Task<InventoryItemResponse> GetInventoryItemById(Guid id)
    {
        TokenHandlerAdder.Add(_httpClient, _httpContextAccessor);
        var response = await _httpClient.GetAsync(InventoryRoute.GetByIdUrl(id));
        await _errorHandler.EnsureSuccessOrThrow(response);

        var result = await response.Content.ReadFromJsonAsync<InventoryItemResponse>();
        return result!;
    }

    public async Task<PageResponse<InventoryItemResponse>> GetInventoryPageByCharacterId(InventoryGetPageByCharacterIdRequire req)
    {
        TokenHandlerAdder.Add(_httpClient, _httpContextAccessor);
        var url = InventoryRoute.GetPageUrl(req.CharacterId, req.Page, req.PageSize);
        var response = await _httpClient.GetAsync(url);
        await _errorHandler.EnsureSuccessOrThrow(response);

        var result = await response.Content.ReadFromJsonAsync<PageResponse<InventoryItemResponse>>();
        return result!;
    }

    public async Task<InventoryItemResponse> AddInventoryItem(InventoryAddItemRequire req)
    {
        TokenHandlerAdder.Add(_httpClient, _httpContextAccessor);
        var response = await _httpClient.PostAsJsonAsync(InventoryRoute.Add, new
        {
            characterId = req.CharacterId,
            artifactId = req.ArtifactId
        });
        await _errorHandler.EnsureSuccessOrThrow(response);

        var result = await response.Content.ReadFromJsonAsync<InventoryItemResponse>();
        return result!;
    }
}
