namespace ArtifactStore.Application.Models.Requires.Balance;

public class AddBalanceWithCurrencyCommonRequire
{
    public Guid CharacterId { get; set; }
    public Guid CurrencyId { get; set; }
    public decimal Amount { get; set; }
}