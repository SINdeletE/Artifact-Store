namespace ArtifactStore.WebApp.Shared.Models.Balance;

public sealed class CharacterCurrency
{
    public Guid BalanceId { get; set; }
    public Guid CurrencyId { get; set; }
    public string Name { get; set; } = "";
    public decimal Amount { get; set; }
}
