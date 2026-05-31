using ArtifactStore.Auth.Application.Models.Requires.Token;

namespace ArtifactStore.Auth.Application.Interfaces.Services;

public interface ITokenService
{
    public string GenerateToken(GenerateTokenRequire req);
}