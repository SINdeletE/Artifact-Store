using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Domain.Tests.Entities;

public class StoreTransactionTests
{
    private static Character CreateCharacter() =>
        new(Guid.NewGuid(), Guid.NewGuid(), "Hero", DateOnly.FromDateTime(DateTime.Today), null);

    private static Balance CreateBalance() =>
        new(Guid.NewGuid(), CreateCharacter(), new Currency(Guid.NewGuid(), "Gold", null), 500m, null);

    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var balance = CreateBalance();
        var artifact = new Artifact(Guid.NewGuid(), "Shield", "Iron shield.", null);
        var storeTransactionType = new StoreTransactionType(Guid.NewGuid(), "purchase");
        var storeTransactionStatus = new StoreTransactionStatus(Guid.NewGuid(), "success");
        var date = new DateTime(2024, 3, 20, 10, 0, 0);

        var tx = new StoreTransaction(Guid.NewGuid(), balance, artifact, 150m, storeTransactionType, storeTransactionStatus, date);

        Assert.Equal(balance, tx.Balance);
        Assert.Equal(artifact, tx.Artifact);
        Assert.Equal(150m, tx.Amount);
        Assert.Equal(storeTransactionType, tx.StoreTransactionType);
        Assert.Equal(storeTransactionStatus, tx.StoreTransactionStatus);
        Assert.Equal(date, tx.TransactionDateTime);
        Assert.NotEqual(Guid.Empty, tx.Id);
    }
}
