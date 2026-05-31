namespace ArtifactStore.Domain.Factories.Balance;

public interface IBalanceFactory
{
    public Entities.Balance Create(Entities.Character character, Entities.Currency currency, decimal amount, DateTimeOffset? deletedAt);
}