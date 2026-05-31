using System.Text.Json;
using ArtifactStore.Application.Common.Models.Filters;
using ArtifactStore.Application.Interfaces.Repositories;
using ArtifactStore.Application.Models.Requires.Character;
using ArtifactStore.Application.Models.Responses;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using ArtifactStore.Domain.Factories.Character;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace ArtifactStore.Application.Services;

public class CharacterService : ICharacterService
{
    ILogger<CharacterService> _logger;
    IDistributedCache _cache;
    
    ICharacterRepository _characterRepository;
    ICharacterFactory _characterFactory;

    public CharacterService(ICharacterRepository characterRepository, ICharacterFactory characterFactory,
        ILogger<CharacterService> logger, IDistributedCache cache)
    {
        _logger = logger;
        _cache = cache;
        
        _characterRepository = characterRepository;
        _characterFactory = characterFactory;
    }
    
    public async Task<Character> AddCharacter(AddCharacterRequire req)
    {
        var character = _characterFactory.Create(req.UserId, req.Name, null);
        _logger.LogInformation("Created character {CharacterId}", character.Id);
        
        var result =  await _characterRepository.InsertAsync(character);
        _logger.LogInformation("Inserted character {CharacterId}", result.Id);
        
        await _cache.RemoveAsync($"characters:account_id:{character.UserId}");
        _logger.LogInformation("Removed cached characters for {AccountId}", character.UserId);
        
        return result;
    }

    public async Task DeleteCharacter(DeleteCharacterRequire req)
    {
        var character = await _characterRepository.GetByIdAsync(req.Id);
        _logger.LogInformation("Fetched character {CharacterId}", character.Id);
        
        if (!req.IsPrivileged && req.AccountId != character.UserId)
        {
            throw new ForbiddenException("Operation is not allowed");
        }
        
        await _characterRepository.DeleteAsync(character);
        _logger.LogInformation("Deleted character {CharacterId}", character.Id);
        
        await _cache.RemoveAsync($"characters:account_id:{character.UserId}");
        _logger.LogInformation("Removed cached characters for {AccountId}", character.UserId);
    }

    public async Task<PageResult<Character>> GetCharacterPage(GetCharacterPageRequire req)
    {
        var characterPage = await _characterRepository.FindAsync(new PageEntityFilter<Character>(req.Page, req.PageSize));
        _logger.LogInformation("Fetched character page");
        
        return new PageResult<Character>(characterPage, characterPage.Count());
    }

    public async Task<PageResult<Character>> GetUserCharacterPage(GetUserCharacterPageRequire req)
    {
        var characterPage = await _characterRepository.FindAsync(new AccountPageCharacterFilter(req.AccountId, req.Page, req.PageSize));
        _logger.LogInformation("Fetched character page");
        
        return new PageResult<Character>(characterPage, characterPage.Count());
    }

    public async Task<IEnumerable<Character>> GetUserCharacters(Guid accountId)
    {
        var cacheKey = $"characters:account_id:{accountId}";
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached is not null)
        {
            _logger.LogInformation("Found cached characters from {CacheKey}", cacheKey);
            
            var deserialized = JsonSerializer.Deserialize<IEnumerable<CharacterResponse>>(cached);
            if (deserialized is not null)
            {
                return deserialized
                    .Select(item => item.Get())
                    .ToList();
            }
        }
        else
        {
            _logger.LogInformation("Not found cached characters from {CacheKey}", cacheKey);
        }
        
        var result = await _characterRepository.FindAsync(new UserCharacterFilter(accountId));
        _logger.LogInformation("Fetched characters for {AccountId}", accountId);
        var toCache = result
            .Select(item => new CharacterResponse(item))
            .ToList();
        
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60),
        };
        await _cache.SetStringAsync(cacheKey, 
            JsonSerializer.Serialize(toCache), options);
        _logger.LogInformation("Added to cache characters to {CacheKey}", cacheKey);
        
        return result;
    }

    public async Task<Character> GetCharacterById(GetCharacterByIdRequire req)
    {
        var result = await _characterRepository.GetByIdAsync(req.Id);
        _logger.LogInformation("Fetched character {CharacterId}", result.Id);
        
        return result;
    }
}