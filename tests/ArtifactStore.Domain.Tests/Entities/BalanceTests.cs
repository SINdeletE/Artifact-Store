using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Domain.Tests.Entities;

public class BalanceTests
{
    private static Character MakeCharacter() =>
        new(Guid.NewGuid(), Guid.NewGuid(), "Hero", DateOnly.FromDateTime(DateTime.Today), null);

    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var character = MakeCharacter();
        var currency = new Currency(Guid.NewGuid(), "Gold", null);

        var balance = new Balance(Guid.NewGuid(), character, currency, 250m, null);

        Assert.Equal(character, balance.Character);
        Assert.Equal(currency, balance.Currency);
        Assert.Equal(250m, balance.Amount);
        Assert.NotEqual(Guid.Empty, balance.Id);
    }


}