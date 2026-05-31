using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Domain.Factories.StoreTransaction;

public interface IStoreTransactionFactory
{
    public Entities.StoreTransaction Create(Entities.Balance balance,
        Entities.Artifact artifact, decimal amount, StoreTransactionType transactionType,
        StoreTransactionStatus transactionStatus);
    
    public Entities.StoreTransaction Create(Guid balanceId, Guid artifactId, decimal amount, 
        Guid transactionTypeId, Guid transactionStatusId);
}