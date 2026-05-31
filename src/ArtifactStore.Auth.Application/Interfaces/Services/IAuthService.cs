using ArtifactStore.Auth.Application.Models.Requires.Auth;

namespace ArtifactStore.Auth.Application.Interfaces.Services;

public interface IAuthService
{
    public Task<string> LoginAsync(AuthLoginRequire req);
    public Task RegisterAsync(AuthRegisterRequire req);
}