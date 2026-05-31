using ArtifactStore.WebApp.Requires;
using ArtifactStore.WebApp.Requires.Auth;
using ArtifactStore.WebApp.Responses;
using ErrorResponse = ArtifactStore.WebApp.Shared.Responses.ErrorResponse;

namespace ArtifactStore.WebApp.Interfaces;

public interface IAccountService
{
    public Task<LoginResponse> Login(LoginRequire req);
    public Task Register(RegisterRequire req);
}
