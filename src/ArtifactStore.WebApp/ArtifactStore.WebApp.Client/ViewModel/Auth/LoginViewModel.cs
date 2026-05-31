using System.ComponentModel.DataAnnotations;
using ArtifactStore.WebApp.Shared.Routes;
using Blazing.Mvvm.ComponentModel;
using System.Net.Http.Json;
using ArtifactStore.WebApp.Shared.Models;
using ArtifactStore.WebApp.Shared.Models.Auth;
using ArtifactStore.WebApp.Shared.Responses;

namespace ArtifactStore.WebApp.Client.ViewModel.Auth;

[ViewModelDefinition(Lifetime = ServiceLifetime.Scoped)]
public sealed class LoginViewModel : ViewModelBase, IDisposable
{
    HttpClient _httpClient;
    
    public LoginModel loginModel { get; set; } = new LoginModel();

    public string? StatusMessage { get; set; }
    
    public LoginViewModel(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> Login()
    {
        var response = await _httpClient.PostAsJsonAsync(AuthRoute.Login, new
        {
            nickname = loginModel.Nickname,
            password = loginModel.Password
        });

        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                StatusMessage = error?.Error ?? "Login failed.";
            }
            catch (Exception)
            {
                StatusMessage = "Login failed with unknown error.";
            }
        }
        
        return response.IsSuccessStatusCode;
    }
}
