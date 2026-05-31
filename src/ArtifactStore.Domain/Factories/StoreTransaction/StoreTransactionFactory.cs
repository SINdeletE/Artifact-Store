using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Domain.Factories.StoreTransaction;

public class StoreTransactionFactory : IStoreTransactionFactory
{
    public Entities.StoreTransaction Create(Entities.Balance balance, Entities.Artifact artifact, decimal amount,
        StoreTransactionType transactionType, StoreTransactionStatus transactionStatus)
    {
        var storeTransactionDate = DateTime.UtcNow;
            
        return new Entities.StoreTransaction(Guid.NewGuid(), balance, artifact, amount, transactionType, transactionStatus, storeTransactionDate);
    }

    public Entities.StoreTransaction Create(Guid balanceId, Guid artifactId, decimal amount, Guid transactionTypeId,
        Guid transactionStatusId)
    {
        var storeTransactionDate = DateTime.UtcNow;
        
        return new Entities.StoreTransaction(Guid.NewGuid(), balanceId, artifactId, amount, transactionTypeId, transactionStatusId, storeTransactionDate);
    }
}