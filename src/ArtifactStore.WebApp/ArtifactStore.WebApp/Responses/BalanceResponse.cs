namespace ArtifactStore.WebApp.Responses;

public class BalanceResponse
{
    public Guid Id { get; set; }
    public Guid CharacterId { get; set; }
    public CharacterResponse? Character { get; set; }
    public Guid CurrencyId { get; set; }
    public CurrencyResponse? Currency { get; set; }
    public decimal Amount { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public uint Version { get; set; }
}
