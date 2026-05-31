using ArtifactStore.Application.Interfaces.Identity;
using ArtifactStore.Application.Interfaces.Tenancy;
using Microsoft.Extensions.Configuration;

namespace ArtifactStore.Infrastructure.Tenancy;

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