namespace ArtifactStore.Application.Models.Requires.Balance;

public class GetBalanceWithCharacterCurrencyRequire
{
    public Guid CharacterId { get; set; }
    public Guid CurrencyId { get; set; }
}