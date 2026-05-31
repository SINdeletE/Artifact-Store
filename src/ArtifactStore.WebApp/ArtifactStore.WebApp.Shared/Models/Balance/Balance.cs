using ArtifactStore.WebApp.Shared.Models.Currency;

namespace ArtifactStore.WebApp.Shared.Models.Balance;

public sealed class Balance
{
    public Guid Id { get; set; }
    public Guid CurrencyId { get; set; }
    public ShopCurrency? Currency { get; set; }
    public decimal Amount { get; set; }
}
