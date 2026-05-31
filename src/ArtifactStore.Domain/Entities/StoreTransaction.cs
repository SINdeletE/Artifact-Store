using ArtifactStore.Domain.Common;

namespace ArtifactStore.Domain.Entities;

public class StoreTransaction : IEntity
{
    public Guid Id { get; private set; }
    
    public Guid BalanceId { get; set; }

    public Balance Balance
    {
        get;
        set
        {
            field = value;
            BalanceId = value.Id;
        }
    }

    public Guid ArtifactId { get; set; }

    public Artifact Artifact
    {
        get;
        set
        {
            field = value;
            ArtifactId = value.Id;
        }
    }

    public decimal Amount { get; set; }
    
    public Guid StoreTransactionTypeId { get; set; }

    public StoreTransactionType StoreTransactionType
    {
        get;
        set
        {
            field = value;
            StoreTransactionTypeId = value.Id;
        }
    }
    
    public Guid StoreTransactionStatusId { get; set; }

    public StoreTransactionStatus StoreTransactionStatus
    {
        get;
        set
        {
            field = value;
            StoreTransactionStatusId = value.Id;
        }
    }

    public DateTime TransactionDateTime { get; set; }
    
    public StoreTransaction() {}
    public StoreTransaction(Guid id, Balance balance, Artifact artifact, decimal amount, 
        StoreTransactionType storeTransactionType, StoreTransactionStatus storeTransactionStatus, DateTime transactionDateTime)
    {
        Id = id;
        Balance = balance;
        Artifact = artifact;
        Amount = amount;
        StoreTransactionType = storeTransactionType;
        StoreTransactionStatus = storeTransactionStatus;
        TransactionDateTime = transactionDateTime;
    }

    public StoreTransaction(Guid id, Guid balanceId, Guid artifactId, decimal amount,
        Guid storeTransactionTypeId, Guid storeTransactionStatusId, DateTime transactionDateTime)
    {
        Id = id;
        BalanceId = balanceId;
        ArtifactId = artifactId;
        Amount = amount;
        StoreTransactionTypeId = storeTransactionTypeId;
        StoreTransactionStatusId = storeTransactionStatusId;
        TransactionDateTime = transactionDateTime;
    }
}