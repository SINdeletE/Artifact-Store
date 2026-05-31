using ArtifactStore.Application.Interfaces.Repositories.Store;
using ArtifactStore.Application.Interfaces.Services.Store;
using ArtifactStore.Application.Models.Requires.Balance;
using ArtifactStore.Application.Models.Requires.Store.Purchase;
using ArtifactStore.Application.Models.Requires.StoreTransaction;
using ArtifactStore.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ArtifactStore.Application.Services.Store;

public class PurchaseService : IPurchaseService
{
    ILogger<PurchaseService> _logger;

    IStoreRepository _storeRepository;
    IBalanceService _balanceService;

    IStoreProcedureRepository _storeProcedureRepository;
    IStoreTransactionService _storeTransactionService;

    public PurchaseService(IStoreRepository storeRepository,
        IStoreProcedureRepository storeProcedureRepository,
        IStoreTransactionService storeTransactionService,
        IBalanceService balanceService,
        ILogger<PurchaseService> logger)
    {
        _logger = logger;

        _balanceService = balanceService;
        _storeRepository = storeRepository;
        _storeTransactionService = storeTransactionService;
        _storeProcedureRepository = storeProcedureRepository;
    }

    public async Task PurchaseStoreItem(PurchaseStoreItemRequire req)
    {
        string statusName = "success";

        StoreItem storeItem = await _storeRepository.GetByIdAsync(req.StoreItemId);
        _logger.LogInformation("Fetched store item {StoreItemId}", req.StoreItemId);

        try
        {
            await _storeProcedureRepository.BuyAsync(req.CharacterId, req.StoreItemId);
            _logger.LogInformation("Purchased artifact {ArtifactId} by character {CharacterId}", storeItem.ArtifactId, req.CharacterId);
        }
        catch (Exception)
        {
            _logger.LogInformation("Can't purchase artifact {ArtifactId} by character {CharacterId}", storeItem.ArtifactId, req.CharacterId);
            
            statusName = "denied";

            throw;
        }
        finally
        {
            var balance = await _balanceService.GetBalanceWithCharacterCurrency(new GetBalanceWithCharacterCurrencyRequire
            {
                CharacterId = req.CharacterId,
                CurrencyId = storeItem.CurrencyId
            });
            
            await _storeTransactionService.AddStoreTransaction(new AddStoreTransactionRequire
            {
                BalanceId = balance.Id,
                ArtifactId = storeItem.ArtifactId,
                Status = statusName,
                Type = "purchase",
                Amount = storeItem.Price * (1 - storeItem.Discount?.Percent ?? 0)
            });
        }
    }
}