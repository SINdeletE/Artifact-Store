using System.Text.Json;
using ArtifactStore.Application.Common.Models.Filters;
using ArtifactStore.Application.Interfaces.Repositories.Store;
using ArtifactStore.Application.Interfaces.Services.Store;
using ArtifactStore.Application.Models.Requires.Store;
using ArtifactStore.Application.Models.Responses;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Factories.Store;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using PurchaseStoreItemRequire = ArtifactStore.Application.Models.Requires.Store.Purchase.PurchaseStoreItemRequire;
using SellStoreItemRequire = ArtifactStore.Application.Models.Requires.Store.Sell.SellStoreItemRequire;

namespace ArtifactStore.Application.Services.Store;

public class StoreService : IStoreService
{
    ILogger<StoreService> _logger;
    IDistributedCache _cache;
    
    private IStoreRepository _storeRepository;
    private IPurchaseService _purchaseService;
    private ISellService _sellService;
    private IStoreItemFactory _storeItemFactory;

    public StoreService(IStoreRepository storeRepository, IPurchaseService purchaseService,
        ISellService sellService, IStoreItemFactory storeItemFactory, ILogger<StoreService> logger,
        IDistributedCache cache)
    {
        _logger = logger;
        _cache = cache;
        
        _storeRepository = storeRepository;
        _purchaseService = purchaseService;
        _sellService = sellService;
        _storeItemFactory = storeItemFactory;
    }
    
    public async Task<StoreItem> AddStoreItem(AddStoreItemRequire req)
    {
        var storeItem = _storeItemFactory.Create(req.Currency, req.Artifact, req.Price);
        _logger.LogInformation("Created store item {StoreItemId}", storeItem.Id);
        
        var result = await _storeRepository.InsertAsync(storeItem);
        _logger.LogInformation("Inserted store item {StoreItemId}", storeItem.Id);
        
        return result;
    }

    public async Task DeleteStoreItem(DeleteStoreItemRequire req)
    {
        var storeItem = await _storeRepository.GetByIdAsync(req.Id);
        _logger.LogInformation("Fetched store item {StoreItemId}", storeItem.Id);
        
        await _storeRepository.DeleteAsync(storeItem);
        _logger.LogInformation("Deleted store item {StoreItemId}", storeItem.Id);
    }

    public async Task UpdateStoreItem(UpdateStoreItemRequire req)
    {
        var storeItem = await _storeRepository.GetByIdAsync(req.Id);
        _logger.LogInformation("Fetched store item {StoreItemId}", storeItem.Id);
        
        storeItem.Currency = req.Currency;
        storeItem.Artifact = req.Artifact;
        storeItem.Price = req.Price;
        storeItem.Version = req.Version;
        
        await _storeRepository.UpdateAsync(storeItem);
        _logger.LogInformation("Updated the store item {StoreItemId}", storeItem.Id);
    }

    public async Task<PageResult<StoreItem>> GetStoreItemPage(GetStoreItemPageRequire req)
    {
        var cacheKey = $"store_item:page:{req.Page}:pagesize:{req.PageSize}";
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached is not null)
        {
            _logger.LogInformation("Found cached store item page from {CacheKey}", cacheKey);
            
            var deserialized = JsonSerializer.Deserialize<PageResult<StoreItemResponse>>(cached);
            if (deserialized is not null)
            {
                return new PageResult<StoreItem>(
                    deserialized.Items
                        .Select(item => item.Get())
                        .ToList(),
                    deserialized.TotalItems);
            }
        }
        else
        {
            _logger.LogInformation("Not found cached store item page from {CacheKey}", cacheKey);
        }
        
        var storeItemPage = await _storeRepository
            .FindAsync(new PageEntityFilter<StoreItem>(req.Page, req.PageSize));
        _logger.LogInformation("Fetched store item page {StoreItemPage}:{StoreItemPageSize}", req.Page, req.PageSize);

        var result = new PageResult<StoreItem>(storeItemPage, storeItemPage.Count());
        var toCache = new PageResult<StoreItemResponse>(
            result.Items
                .Select(item => new StoreItemResponse(item))
                .ToList(),
            result.TotalItems);
        
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5),
        };
        await _cache.SetStringAsync(cacheKey, 
            JsonSerializer.Serialize(toCache), options);
        _logger.LogInformation("Added to cache store item page to {CacheKey}", cacheKey);
        
        return result;
    }

    public Task<StoreItem> GetStoreItemById(Guid storeItemId)
    {
        var storeItem = _storeRepository.GetByIdAsync(storeItemId);
        _logger.LogInformation("Fetched store item {StoreItemId}", storeItem.Id);
        
        return storeItem;
    }

    public async Task PurchaseStoreItem(PurchaseStoreItemRequire req)
    {
        await _purchaseService.PurchaseStoreItem(req);
        _logger.LogInformation("Purchased store item {StoreItemId} by character {CharacterId}", req.StoreItemId, req.CharacterId);
    }
    
    public async Task SellStoreItem(SellStoreItemRequire req)
    {
        await _sellService.SellStoreItem(req);
        _logger.LogInformation("Sold inventory item {InventoryItemId} using currency {CurrencyId}", req.InventoryItemId, req.CurrencyId);
    }
}