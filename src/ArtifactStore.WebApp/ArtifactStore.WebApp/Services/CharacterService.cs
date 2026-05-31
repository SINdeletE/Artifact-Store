using System.Net.Http.Headers;
using ArtifactStore.WebApp.Error;
using ArtifactStore.WebApp.HandlerAdders;
using ArtifactStore.WebApp.Interfaces;
using ArtifactStore.WebApp.Requires.Character;
using ArtifactStore.WebApp.Responses;
using ArtifactStore.WebApp.Shared.Http;
using ArtifactStore.WebApp.Shared.Routes;

namespace ArtifactStore.WebApp.Services;

public class CharacterService : ICharacterService
{
    IErrorHandler _errorHandler;
    HttpClient _httpClient;
    IHttpContextAccessor _httpContextAccessor;

    public CharacterService(IHttpClientFactory factory, IErrorHandler errorHandler, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = factory.CreateClient(HttpClientName.Main);
        _errorHandler = errorHandler;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CharacterResponse> GetCharacterById(Guid id)
    {
        TokenHandlerAdder.Add(_httpClient, _httpContextAccessor);
        var response = await _httpClient.GetAsync(CharacterRoute.GetByIdUrl(id));
        await _errorHandler.EnsureSuccessOrThrow(response);

        var result = await response.Content.ReadFromJsonAsync<CharacterResponse>();
        return result!;
    }

    public async Task<PageResponse<CharacterResponse>> GetCharacterPage(CharacterGetPageRequire req)
    {
        TokenHandlerAdder.Add(_httpClient, _httpContextAccessor);
        var response = await _httpClient.GetAsync(ServiceUrl.WithPage(CharacterRoute.GetPage, req.Page, req.PageSize));
        await _errorHandler.EnsureSuccessOrThrow(response);

        var result = await response.Content.ReadFromJsonAsync<PageResponse<CharacterResponse>>();
        return result!;
    }

    public async Task<PageResponse<CharacterResponse>> GetUserCharacterPage(CharacterGetUserPageRequire req)
    {
        TokenHandlerAdder.Add(_httpClient, _httpContextAccessor);
        var response = await _httpClient.GetAsync(ServiceUrl.WithPage(CharacterRoute.GetUserPage, req.Page, req.PageSize));
        await _errorHandler.EnsureSuccessOrThrow(response);

        var result = await response.Content.ReadFromJsonAsync<PageResponse<CharacterResponse>>();
        return result!;
    }
    
    public async Task<IEnumerable<CharacterResponse>> GetUserCharacters()
    {
        TokenHandlerAdder.Add(_httpClient, _httpContextAccessor);
        var response = await _httpClient.GetAsync(CharacterRoute.GetUserAll);
        await _errorHandler.EnsureSuccessOrThrow(response);

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<CharacterResponse>>();
        return result!;
    }

    public async Task DeleteCharacter(Guid id)
    {
        TokenHandlerAdder.Add(_httpClient, _httpContextAccessor);
        var response = await _httpClient.DeleteAsync(CharacterRoute.DeleteUrl(id));
        await _errorHandler.EnsureSuccessOrThrow(response);
    }

    public async Task<CharacterResponse> AddCharacter(CharacterAddRequire req)
    {
        TokenHandlerAdder.Add(_httpClient, _httpContextAccessor);
        var response = await _httpClient.PostAsJsonAsync(CharacterRoute.Add, new
        {
            name = req.Name
        });
        await _errorHandler.EnsureSuccessOrThrow(response);

        var result = await response.Content.ReadFromJsonAsync<CharacterResponse>();
        return result!;
    }
}
