using ArtifactStore.Auth.Application.Interfaces.Identity;
using ArtifactStore.Auth.Application.Interfaces.Tenancy;
using Microsoft.Extensions.Configuration;

namespace ArtifactStore.Auth.Infrastructure.Tenancy;

public class ConnectionStringHolder : IConnectionStringHolder
{
    private string _connectionString;

    public ConnectionStringHolder(IIdentityHolder roleHolder)
    {
        _connectionString = roleHolder.Role switch
        {
            "admin" => "Admin",
            "moderator" => "Moderator",
            "default_user" => "DefaultUser",
            _ => throw new ArgumentException($"Unknown role: {roleHolder.Role}")
        };
    }

    public string GetConnectionString()
    {
        return _connectionString;
    }
}