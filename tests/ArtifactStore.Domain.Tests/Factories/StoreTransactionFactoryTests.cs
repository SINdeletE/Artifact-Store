using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Factories.StoreTransaction;

namespace ArtifactStore.Domain.Tests.Factories;

public class StoreTransactionFactoryTests
{
    private readonly StoreTransactionFactory _factory = new();

    private static Balance CreateBalance() =>
        new(Guid.NewGuid(), new Character(Guid.NewGuid(), Guid.NewGuid(), "Hero", DateOnly.FromDateTime(DateTime.Today), null),
            new Currency(Guid.NewGuid(), "Gold", null), 500m, null);

    [Fact]
    public void Create_SetsPropertiesCorrectly()
    {
        var balance = CreateBalance();
        var artifact = new Artifact(Guid.NewGuid(), "Shield", "Iron shield.", null);
        var storeTransactionType = new StoreTransactionType(Guid.NewGuid(), "purchase");
        var storeTransactionStatus = new StoreTransactionStatus(Guid.NewGuid(), "success");

        var tx = _factory.Create(balance, artifact, 200m, storeTransactionType, storeTransactionStatus);

        Assert.Equal(balance, tx.Balance);
        Assert.Equal(artifact, tx.Artifact);
        Assert.Equal(200m, tx.Amount);
        Assert.Equal(storeTransactionType, tx.StoreTransactionType);
        Assert.Equal(storeTransactionStatus, tx.StoreTransactionStatus);
        Assert.NotEqual(default, tx.TransactionDateTime);
        Assert.NotEqual(Guid.Empty, tx.Id);
    }
}
