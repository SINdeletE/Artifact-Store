using ArtifactStore.WebApp.Responses;

namespace ArtifactStore.WebApp.Requires.Balance;

public class BalanceUpdateRequire
{
    public Guid Id { get; set; }
    public required CharacterResponse Character { get; set; }
    public required CurrencyResponse Currency { get; set; }
    public decimal Amount { get; set; }
    public uint Version { get; set; }
}
