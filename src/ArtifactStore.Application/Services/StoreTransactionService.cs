using System.Text.Json;
using ArtifactStore.Application.Common.Models.Filters;
using ArtifactStore.Application.Interfaces.Repositories.Transactions;
using ArtifactStore.Application.Models.Requires.StoreTransaction;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Factories.StoreTransaction;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace ArtifactStore.Application.Services;

public class StoreTransactionService : IStoreTransactionService
{
    ILogger<StoreTransactionService> _logger;
    
    IStoreTransactionRepository _storeTransactionRepository;
    IStoreTransactionTypeRepository _storeTransactionTypeRepository;
    IStoreTransactionStatusRepository _storeTransactionStatusRepository;
    IStoreTransactionFactory _storeTransactionFactory;
    
    IDistributedCache _cache;

    public StoreTransactionService(IStoreTransactionRepository storeTransactionRepository,
        IStoreTransactionTypeRepository storeTransactionTypeRepository,
        IStoreTransactionStatusRepository storeTransactionStatusRepository,
        IStoreTransactionFactory storeTransactionFactory, IDistributedCache cache, ILogger<StoreTransactionService> logger)
    {
        _logger = logger;
        
        _storeTransactionRepository = storeTransactionRepository;
        _storeTransactionTypeRepository = storeTransactionTypeRepository;
        _storeTransactionStatusRepository = storeTransactionStatusRepository;
        _storeTransactionFactory = storeTransactionFactory;
        _cache = cache;
    }
    
    public async Task<StoreTransaction> AddStoreTransaction(AddStoreTransactionRequire req)
    {
        var transactionStatusEnumerable =
            await _storeTransactionStatusRepository.FindAsync(new StoreTransactionStatusFilter(req.Status));
        var transactionStatus = transactionStatusEnumerable.First();
        _logger.LogInformation("Fetched store transaction status {StoreTransactionStatusId}", transactionStatus.Id);
        
        var transactionTypeEnumerable =
            await _storeTransactionTypeRepository.FindAsync(new StoreTransactionTypeFilter("purchase"));
        var transactionType = transactionTypeEnumerable.First();
        _logger.LogInformation("Fetched store transaction type {StoreTransactionTypeId}", transactionType.Id);
        
        var storeTransaction = _storeTransactionFactory.Create(req.BalanceId,
            req.ArtifactId, req.Amount, transactionType.Id,  transactionStatus.Id);
        _logger.LogInformation("Created store transaction {StoreTransactionId}", storeTransaction.Id);

        var result = await _storeTransactionRepository.InsertAsync(storeTransaction);
        _logger.LogInformation("Inserted store transaction {StoreTransactionId}", result.Id);
        
        return result;
    }

    public async Task<PageResult<StoreTransaction>> GetStoreTransactionPage(GetStoreTransactionPageRequire req)
    {
        var cacheKey = $"store_transaction:page:{req.Page}:pagesize:{req.PageSize}";
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached is not null)
        {
            _logger.LogInformation("Found cached store transaction page from {Cachekey}", cacheKey);
            
            var deserialized = JsonSerializer.Deserialize<PageResult<StoreTransaction>>(cached);
            if (deserialized is not null)
                return deserialized;
        }
        else
        {
            _logger.LogInformation("Not found cached store transaction page from {Cachekey}", cacheKey);
        }
        
        var storeTransactionPage = await _storeTransactionRepository
            .FindAsync(new PageEntityFilter<StoreTransaction>(req.Page, req.PageSize));
        _logger.LogInformation("Fetched store transaction page");
        
        var result = new PageResult<StoreTransaction>(storeTransactionPage, storeTransactionPage.Count());

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60),
        };
        await _cache.SetStringAsync(cacheKey, 
            JsonSerializer.Serialize(result), options);
        _logger.LogInformation("Added to cache store transaction page to {Cachekey}", cacheKey);
            
        return result;
    }
}