using ArtifactStore.Application.Common.Models.Filters;
using ArtifactStore.Application.Interfaces.Repositories.Store;
using ArtifactStore.Application.Interfaces.Services.Store;
using ArtifactStore.Application.Models.Requires.Balance;
using ArtifactStore.Application.Models.Requires.Inventory;
using ArtifactStore.Application.Models.Requires.Store.Sell;
using ArtifactStore.Application.Models.Requires.StoreTransaction;
using ArtifactStore.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ArtifactStore.Application.Services.Store;

public class SellService : ISellService
{
    ILogger<SellService> _logger;
    
    IStoreProcedureRepository _storeProcedureRepository;
    IStoreTransactionService _storeTransactionService;
    IInventoryService _inventoryService;
    IBalanceService _balanceService;

    public SellService(IStoreProcedureRepository storeProcedureRepository,
        IStoreTransactionService storeTransactionService,
        IInventoryService inventoryService,
        IBalanceService balanceService,
        ILogger<SellService> logger)
    {
        _logger = logger;
        
        _storeProcedureRepository = storeProcedureRepository;
        _storeTransactionService = storeTransactionService;
        _inventoryService = inventoryService;
        _balanceService = balanceService;
    }

    public async Task SellStoreItem(SellStoreItemRequire req)
    {
        var inventoryItem = await _inventoryService.GetInventoryItemById(new GetInventoryItemById{ Id = req.InventoryItemId });
        var balance = await _balanceService.GetBalanceWithCharacterCurrency(new GetBalanceWithCharacterCurrencyRequire
        {
            CharacterId = inventoryItem.CharacterId,
            CurrencyId = req.CurrencyId,
        });

        string statusName = "success";

        try
        {
            await _storeProcedureRepository.SellAsync(inventoryItem.Id, req.CurrencyId, req.Price);
            _logger.LogInformation("Sold artifact {ArtifactId} using balance {BalanceId}", inventoryItem.ArtifactId, balance.Id);
        }
        catch (Exception)
        {
            _logger.LogInformation("Can't sell artifact {ArtifactId} using balance {BalanceId}", inventoryItem.ArtifactId, balance.Id);
            
            statusName = "denied";

            throw;
        }
        finally
        {
            await _storeTransactionService.AddStoreTransaction(new AddStoreTransactionRequire
            {
                BalanceId = balance.Id,
                ArtifactId = inventoryItem.ArtifactId,
                Status = statusName,
                Type = "sell",
                Amount = req.Price
            });
        }
    }
}