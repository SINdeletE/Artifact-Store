using ArtifactStore.Application.Interfaces.Repositories;
using ArtifactStore.Application.Models.Requires.Currency;
using ArtifactStore.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ArtifactStore.Application.Services;

public class CurrencyService : ICurrencyService
{
    ILogger<CurrencyService> _logger;
    
    ICurrencyRepository _currencyRepository;

    public CurrencyService(ICurrencyRepository currencyRepository, ILogger<CurrencyService> logger)
    {
        _logger = logger;
        
        _currencyRepository = currencyRepository;
    }
    
    public async Task<Currency> GetCurrencyById(GetCurrencyByIdRequire req)
    {
        var result = await _currencyRepository.GetByIdAsync(req.Id);
        _logger.LogInformation("Fetched currency {CurrencyId}", result.Id);
        
        return result;
    }
}