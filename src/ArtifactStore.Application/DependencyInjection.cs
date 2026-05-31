using ArtifactStore.Application.Interfaces.Services.Store;
using ArtifactStore.Application.Services;
using ArtifactStore.Application.Services.Store;
using ArtifactStore.Domain.Factories.Artifact;
using ArtifactStore.Domain.Factories.Balance;
using ArtifactStore.Domain.Factories.Character;
using ArtifactStore.Domain.Factories.Discount;
using ArtifactStore.Domain.Factories.Inventory;
using ArtifactStore.Domain.Factories.Store;
using ArtifactStore.Domain.Factories.StoreTransaction;
using Microsoft.Extensions.DependencyInjection;

namespace ArtifactStore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IArtifactService, ArtifactService>();
        services.AddScoped<IBalanceService, BalanceService>();
        services.AddScoped<ICharacterService, CharacterService>();
        services.AddScoped<ICurrencyService, CurrencyService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IStoreTransactionService, StoreTransactionService>();
        
        services.AddScoped<IArtifactFactory, ArtifactFactory>();
        services.AddScoped<IBalanceFactory, BalanceFactory>();
        services.AddScoped<ICharacterFactory, CharacterFactory>();
        services.AddScoped<IInventoryItemFactory, InventoryItemFactory>();
        services.AddScoped<IStoreItemFactory, StoreItemFactory>();
        services.AddScoped<IStoreTransactionFactory, StoreTransactionFactory>();

        services.AddScoped<IPurchaseService, PurchaseService>();
        services.AddScoped<ISellService, SellService>();
        services.AddScoped<IStoreService, StoreService>();
        
        services.AddScoped<IDiscountService, DiscountService>();
        services.AddScoped<IDiscountFactory, DiscountFactory>();
        
        return services;
    }
}