using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ArtifactStore.Auth.Application.Interfaces.Services;
using ArtifactStore.Auth.Application.Models.Requires.Token;
using ArtifactStore.Auth.Domain.Exceptions;
using Microsoft.IdentityModel.Tokens;

namespace ArtifactStore.Auth.API.Services;

public class JwtTokenService : ITokenService
{
    string _secretKey;

    public JwtTokenService(IConfiguration configuration)
    {
        _secretKey = configuration.GetSection("SecretKey").Value;
        if (string.IsNullOrEmpty(_secretKey))
        {
            throw new InternalServerErrorException("Token generation Failed: Missing Secret Key");
        }
    }
    public string GenerateToken(GenerateTokenRequire req)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new Dictionary<string, object>
        {
            { "name", req.Account.Nickname },
            { "role", req.Account.AccountRole.Name },
            { JwtRegisteredClaimNames.Sub, req.Account.Id.ToString() }
        };
        
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = "web-server",
            Audience = "web",
            Claims = claims,
            Expires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(30)),
            SigningCredentials = credentials
        };
        
        SecurityToken token = tokenHandler.CreateToken(descriptor);
        
        return tokenHandler.WriteToken(token);
    }
}