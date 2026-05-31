using System.Text.Json;
using ArtifactStore.Application.Common.Models.Filters;
using ArtifactStore.Application.Interfaces.Repositories;
using ArtifactStore.Application.Models.Requires.Balance;
using ArtifactStore.Application.Models.Requires.Character;
using ArtifactStore.Application.Models.Requires.Currency;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using ArtifactStore.Domain.Factories.Balance;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace ArtifactStore.Application.Services;

public class BalanceService : IBalanceService
{
    ILogger<BalanceService> _logger;
    IDistributedCache _cache;
    
    IBalanceRepository _balanceRepository;
    ICharacterService _characterService;
    ICurrencyService _currencyService;
    
    IBalanceFactory _balanceFactory;

    public BalanceService(IBalanceRepository balanceRepository, IBalanceFactory balanceFactory,
        ICharacterService characterService, ICurrencyService currencyService, ILogger<BalanceService> logger,
        IDistributedCache cache)
    {
        _logger = logger;
        _cache = cache;
        
        _balanceRepository = balanceRepository;
        _characterService = characterService;
        _currencyService = currencyService;
        
        _balanceFactory = balanceFactory;
    }
    
    public async Task<Balance> GetBalanceById(GetBalanceByIdRequire req)
    {
        var result =  await _balanceRepository.GetByIdAsync(req.Id);
        _logger.LogInformation("Fetched balance {BalanceId}", result.Id);
        
        return result;
    }

    public async Task UpdateBalance(UpdateBalanceRequire req)
    {
        var balance = await _balanceRepository.GetByIdAsync(req.Id);
        _logger.LogInformation("Fetched balance {BalanceId}", balance.Id);
        
        balance.Character = req.Character;
        balance.Currency = req.Currency;
        balance.Amount = req.Amount;
        balance.Version = req.Version;
        
        await _balanceRepository.UpdateAsync(balance);
        _logger.LogInformation("Updated balance {BalanceId}", balance.Id);
        
        await _cache.RemoveAsync($"balances:character_id:{req.Id}");
        _logger.LogInformation("Removed cached balances for {CharacterId}", req.Id);
    }

    public async Task<Balance> AddBalanceWithCurrency(AddBalanceWithCurrencyRequire req)
    {
        var character = await _characterService.GetCharacterById(new GetCharacterByIdRequire{Id = req.CharacterId});
        var currency = await _currencyService.GetCurrencyById(new GetCurrencyByIdRequire{ Id = req.CurrencyId });

        if (!req.IsPrivileged && character.UserId != req.AccountId)
        {
            throw new ForbiddenException("Operation is not allowed");
        }
        
        var balance = _balanceFactory.Create(character, currency, req.Amount, null);
        _logger.LogInformation("Created balance {BalanceId}", balance.Id);
        
        var result = await _balanceRepository.InsertAsync(balance);
        _logger.LogInformation("Inserted balance {BalanceId}", result.Id);
        
        await _cache.RemoveAsync($"balances:character_id:{character.Id}");
        _logger.LogInformation("Removed cached balances for {CharacterId}", character.Id);
        
        return result;
    }

    public async Task<Balance> GetBalanceWithCharacterCurrency(GetBalanceWithCharacterCurrencyRequire req)
    {
        var character = await _characterService.GetCharacterById(new GetCharacterByIdRequire
        {
            Id = req.CharacterId
        });
        var currency = await _currencyService.GetCurrencyById(new GetCurrencyByIdRequire
        {
            Id = req.CurrencyId
        });
        
        try
        {
            var balanceEnumerable =
                await _balanceRepository.FindAsync(new BalanceCharacterCurrencyFilter(character.Id, currency.Id));
            
            return balanceEnumerable.First();
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException("Balance not found");
        }
    }

    public async Task<IEnumerable<Balance>> GetCharacterBalances(Guid characterId)
    {
        var cacheKey = $"balances:character_id:{characterId}";
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached is not null)
        {
            _logger.LogInformation("Found cached balances from {CacheKey}", cacheKey);
            
            var deserialized = JsonSerializer.Deserialize<IEnumerable<Balance>>(cached);
            if (deserialized is not null)
                return deserialized;
        }
        else
        {
            _logger.LogInformation("Not found cached balances from {CacheKey}", cacheKey);
        }
        
        var result = await _balanceRepository.FindAsync(new BalancesCharacterFilter(characterId));
        
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(3),
        };
        await _cache.SetStringAsync(cacheKey, 
            JsonSerializer.Serialize(result), options);
        _logger.LogInformation("Added to cache characters to {CacheKey}", cacheKey);
        
        return result;
    }
}