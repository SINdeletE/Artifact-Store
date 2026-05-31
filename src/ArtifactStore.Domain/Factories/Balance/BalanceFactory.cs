namespace ArtifactStore.Domain.Factories.Balance;

public class BalanceFactory : IBalanceFactory
{
    public Entities.Balance Create(Entities.Character character, Entities.Currency currency, decimal amount, DateTimeOffset? deletedAt)
    {
        return new Entities.Balance(Guid.NewGuid(), character, currency, amount, deletedAt);
    }
}