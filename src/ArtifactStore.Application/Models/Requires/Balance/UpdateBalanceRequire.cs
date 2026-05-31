namespace ArtifactStore.Application.Models.Requires.Balance;

public class UpdateBalanceRequire
{
    public Guid Id { get; set; }
    public required Domain.Entities.Character Character { get; set; }
    public required Domain.Entities.Currency Currency { get; set; }
    public decimal Amount { get; set; }
    public uint Version { get; set; }
}