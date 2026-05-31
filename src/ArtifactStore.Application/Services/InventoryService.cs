using System.Text.Json;
using ArtifactStore.Application.Common.Models.Filters;
using ArtifactStore.Application.Interfaces.Repositories;
using ArtifactStore.Application.Models.Requires.Artifact;
using ArtifactStore.Application.Models.Requires.Character;
using ArtifactStore.Application.Models.Requires.Inventory;
using ArtifactStore.Application.Models.Responses;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using ArtifactStore.Domain.Factories.Inventory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace ArtifactStore.Application.Services;

public class InventoryService : IInventoryService
{
    ILogger<InventoryService> _logger;
    IDistributedCache _cache;
    
    IInventoryRepository _inventoryRepository;
    ICharacterService _characterService;
    IArtifactService _artifactService;
    
    IInventoryItemFactory _inventoryItemFactory;

    public InventoryService(IInventoryRepository inventoryRepository, ICharacterService characterService, 
        IArtifactService artifactService, IInventoryItemFactory inventoryItemFactory, ILogger<InventoryService> logger,
        IDistributedCache cache)
    {
        _logger = logger;
        _cache = cache;
        
        _inventoryRepository = inventoryRepository;
        _characterService = characterService;
        _artifactService = artifactService;
        
        _inventoryItemFactory = inventoryItemFactory;
    }
    
    public async Task<InventoryItem> GetInventoryItemById(GetInventoryItemById req)
    {
        var cacheKey = $"inventory_item:{req.Id}";
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached is not null)
        {
            _logger.LogInformation("Found cached inventory item from {CacheKey}", cacheKey);
            
            var deserialized = JsonSerializer.Deserialize<InventoryItem>(cached);
            if (deserialized is not null)
                return deserialized;
        }
        else
        {
            _logger.LogInformation("Not found cached inventory item from {CacheKey}", cacheKey);
        }
        
        var result = await _inventoryRepository.GetByIdAsync(req.Id);
        _logger.LogInformation("Fetched inventory item {InventoryItemId}", result.Id);
        
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60),
        };
        await _cache.SetStringAsync(cacheKey, 
            JsonSerializer.Serialize(result), options);
        _logger.LogInformation("Added to cache inventory item to {CacheKey}", cacheKey);
        
        return result;
    }

    public async Task DeleteInventoryItem(DeleteInventoryItemRequire req)
    {
        var inventoryItem = await _inventoryRepository.GetByIdAsync(req.Id);
        _logger.LogInformation("Fetched inventory item {InventoryItemId}", inventoryItem.Id);

        if (!req.IsPrivileged && req.AccountId != inventoryItem.Character.UserId)
        {
            throw new ForbiddenException("Operation is not allowed");
        }

        await _inventoryRepository.DeleteAsync(inventoryItem);
        _logger.LogInformation("Deleted inventory item {InventoryItemId}", inventoryItem.Id);
        
        var cacheKey = $"inventory_item:{req.Id}";
        await _cache.RemoveAsync(cacheKey);
        _logger.LogInformation("Removed cached inventory item from {CacheKey}", cacheKey);
    }

    public async Task<PageResult<InventoryItem>> GetInventoryPageByCharacterId(GetInventoryPageByCharacterIdRequire req)
    {
        var cacheKey = $"inventory_item:character_id:{req.CharacterId}:page:{req.Page}:pagesize:{req.PageSize}";
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached is not null)
        {
            _logger.LogInformation("Found cached inventory item page from {CacheKey}", cacheKey);
            
            var deserialized = JsonSerializer.Deserialize<PageResult<InventoryItemResponse>>(cached);
            if (deserialized is not null)
            {
                return new PageResult<InventoryItem>(
                    deserialized.Items
                        .Select(item => item.Get())
                        .ToList(),
                    deserialized.TotalItems);
            }
        }
        else
        {
            _logger.LogInformation("Not found cached inventory item page from {CacheKey}", cacheKey);
        }
        
        var inventoryPage = await _inventoryRepository.FindAsync(new InventoryPageFilter(req.CharacterId, req.Page, req.PageSize));
        _logger.LogInformation("Fetched inventory item page");
        var result = new PageResult<InventoryItem>(inventoryPage, inventoryPage.Count());
        var toCache = new PageResult<InventoryItemResponse>(
            result.Items
                .Select(item => new InventoryItemResponse(item))
                .ToList(),
            result.TotalItems);
        
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(7),
        };
        await _cache.SetStringAsync(cacheKey, 
            JsonSerializer.Serialize(toCache), options);
        _logger.LogInformation("Added to cache inventory item page to {CacheKey}", cacheKey);
        
        return result;
    }

    public async Task<InventoryItem> AddInventoryItem(AddInventoryItemRequire req)
    {
        var character = await _characterService.GetCharacterById(new GetCharacterByIdRequire{ Id = req.CharacterId });
        var artifact = await _artifactService.GetArtifactById(new GetArtifactByIdRequire{ Id = req.ArtifactId });
        
        var inventoryItem = _inventoryItemFactory.Create(character, artifact);
        _logger.LogInformation("Created inventory item {InventoryItemId}", inventoryItem.Id);
        
        var result = await _inventoryRepository.InsertAsync(inventoryItem);
        _logger.LogInformation("Inserted inventory item {InventoryItemId}", result.Id);
        
        return result;
    }
}
