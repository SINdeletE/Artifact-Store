using ArtifactStore.WebApp.Error;
using ArtifactStore.WebApp.Interfaces;
using ArtifactStore.WebApp.Requires.Artifact;
using ArtifactStore.WebApp.Responses;
using ArtifactStore.WebApp.Shared.Http;
using ArtifactStore.WebApp.Shared.Routes;

namespace ArtifactStore.WebApp.Services;

public class ArtifactService : IArtifactService
{
    IErrorHandler _errorHandler;
    HttpClient _httpClient;

    public ArtifactService(IHttpClientFactory factory, IErrorHandler errorHandler)
    {
        _httpClient = factory.CreateClient(HttpClientName.Main);
        _errorHandler = errorHandler;
    }

    public async Task<ArtifactResponse> AddArtifact(ArtifactAddRequire req)
    {
        var response = await _httpClient.PostAsJsonAsync(ArtifactRoute.Add, new
        {
            name = req.Name,
            description = req.Description
        });
        await _errorHandler.EnsureSuccessOrThrow(response);

        var result = await response.Content.ReadFromJsonAsync<ArtifactResponse>();
        return result!;
    }

    public async Task UpdateArtifact(ArtifactUpdateRequire req)
    {
        var response = await _httpClient.PutAsJsonAsync(ArtifactRoute.Update, new
        {
            id = req.Id,
            name = req.Name,
            description = req.Description,
            version = req.Version
        });
        await _errorHandler.EnsureSuccessOrThrow(response);
    }

    public async Task<ArtifactResponse> GetArtifactById(Guid id)
    {
        var response = await _httpClient.GetAsync(ArtifactRoute.GetByIdUrl(id));
        await _errorHandler.EnsureSuccessOrThrow(response);

        var result = await response.Content.ReadFromJsonAsync<ArtifactResponse>();
        return result!;
    }
}
