namespace ArtifactStore.WebApp.Requires.Balance;

public class BalanceAddWithCurrencyRequire
{
    public Guid CharacterId { get; set; }
    public Guid CurrencyId { get; set; }
    public decimal Amount { get; set; }
}
