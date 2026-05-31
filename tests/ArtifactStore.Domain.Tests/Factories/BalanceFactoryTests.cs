using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Factories.Balance;

namespace ArtifactStore.Domain.Tests.Factories;

public class BalanceFactoryTests
{
    private readonly BalanceFactory _factory = new();

    private static Character MakeCharacter() =>
        new(Guid.NewGuid(), Guid.NewGuid(), "Hero", DateOnly.FromDateTime(DateTime.Today), null);

    [Fact]
    public void Create_SetsPropertiesCorrectly()
    {
        var character = MakeCharacter();
        var currency = new Currency(Guid.NewGuid(), "Gold", null);

        var balance = _factory.Create(character, currency, 100.50m, null);

        Assert.Equal(character, balance.Character);
        Assert.Equal(currency, balance.Currency);
        Assert.Equal(100.50m, balance.Amount);
        Assert.NotEqual(Guid.Empty, balance.Id);
    }


}
