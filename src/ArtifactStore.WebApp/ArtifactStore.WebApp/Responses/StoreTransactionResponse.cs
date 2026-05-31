namespace ArtifactStore.WebApp.Responses;

public class StoreTransactionResponse
{
    public Guid Id { get; set; }
    public Guid BalanceId { get; set; }
    public BalanceResponse? Balance { get; set; }
    public Guid ArtifactId { get; set; }
    public ArtifactResponse? Artifact { get; set; }
    public decimal Amount { get; set; }
    public Guid StoreTransactionTypeId { get; set; }
    public StoreTransactionTypeResponse? StoreTransactionType { get; set; }
    public Guid StoreTransactionStatusId { get; set; }
    public StoreTransactionStatusResponse? StoreTransactionStatus { get; set; }
    public DateTime TransactionDateTime { get; set; }
}
