using ArtifactStore.WebApp.Error;
using ArtifactStore.WebApp.Interfaces;
using ArtifactStore.WebApp.Requires;
using ArtifactStore.WebApp.Requires.Auth;
using ArtifactStore.WebApp.Responses;
using ArtifactStore.WebApp.Shared.Http;
using ArtifactStore.WebApp.Shared.Routes;

namespace ArtifactStore.WebApp.Services;

public class AccountService : IAccountService
{
    IErrorHandler _errorHandler;
    HttpClient _httpClient;

    public AccountService(IHttpClientFactory factory, IErrorHandler errorHandler)
    {
        _httpClient = factory.CreateClient(HttpClientName.Auth);
        _errorHandler = errorHandler;
    }
    
    public async Task<LoginResponse> Login(LoginRequire req)
    {
        var response = await _httpClient.PostAsJsonAsync(AuthRoute.Login, new
        {
            nickname = req.Nickname,
            password = req.Password
        });
        await _errorHandler.EnsureSuccessOrThrow(response);
        
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return result!;
    }

    public async Task Register(RegisterRequire req)
    {
        var response = await _httpClient.PostAsJsonAsync(AuthRoute.Register, new
        {
            nickname = req.Nickname,
            password = req.Password,
            email = req.Email
        });
        await _errorHandler.EnsureSuccessOrThrow(response);
    }
}