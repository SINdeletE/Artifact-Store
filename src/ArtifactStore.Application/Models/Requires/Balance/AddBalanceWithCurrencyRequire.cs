namespace ArtifactStore.Application.Models.Requires.Balance;

public class AddBalanceWithCurrencyRequire
{
    public Guid CharacterId { get; set; }
    public Guid CurrencyId { get; set; }
    public decimal Amount { get; set; }
    
    public Guid AccountId { get; set; }
    public bool IsPrivileged { get; set; } = false;
}