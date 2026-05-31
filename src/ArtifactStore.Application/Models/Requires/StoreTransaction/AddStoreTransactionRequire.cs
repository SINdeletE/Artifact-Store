using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Models.Requires.StoreTransaction;

public class AddStoreTransactionRequire
{
    public Guid BalanceId { get; set; }
    public Guid ArtifactId { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}