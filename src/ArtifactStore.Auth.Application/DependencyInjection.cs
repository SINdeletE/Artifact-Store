using ArtifactStore.Auth.Application.Interfaces.Services;
using ArtifactStore.Auth.Application.Services;
using ArtifactStore.Auth.Domain.Factories.Account;
using Microsoft.Extensions.DependencyInjection;

namespace ArtifactStore.Auth.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IAuthService, AuthService>();
        
        services.AddScoped<IAccountFactory, AccountFactory>();
        
        return services;
    }
}