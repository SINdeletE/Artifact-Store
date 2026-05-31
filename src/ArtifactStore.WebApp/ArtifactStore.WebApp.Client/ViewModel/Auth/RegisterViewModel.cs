using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using ArtifactStore.WebApp.Shared.Models.Auth;
using ArtifactStore.WebApp.Shared.Responses;
using ArtifactStore.WebApp.Shared.Routes;
using Blazing.Mvvm.ComponentModel;

namespace ArtifactStore.WebApp.Client.ViewModel.Auth;

public class RegisterViewModel : ViewModelBase
{
    public RegisterModel RegisterModel = new RegisterModel();
    private HttpClient _httpClient;
    
    public string? StatusMessage = null;
    
    public RegisterViewModel(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<bool> Register()
    {
        var response = await _httpClient.PostAsJsonAsync(AuthRoute.Register, new
        {
            nickname = RegisterModel.Nickname,
            password = RegisterModel.Password,
            email = RegisterModel.Email
        });
        
        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                StatusMessage = error?.Error ?? "Registration failed.";
            }
            catch (Exception e)
            {
                StatusMessage = "Registration failed with unknown error.";
            }
        }
        
        return response.IsSuccessStatusCode;
    }
}