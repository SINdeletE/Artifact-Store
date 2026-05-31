using ArtifactStore.Application.Common.Models.Filters;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Infrastructure.Repositories;
using ArtifactStore.Infrastructure.Repositories.Transaction;

namespace ArtifactStore.Infrastructure.Tests.Repositories;

[Collection("Database")]
public class StoreTransactionRepositoryTests(DatabaseFixture fixture)
{
    [Fact]
    public async Task GetById_ReturnsTransaction_WhenExists()
    {
        await using var ctx = fixture.CreateContext();

        var character = new Character(Guid.NewGuid(), fixture.SeedAccountId, "TxHero", DateOnly.FromDateTime(DateTime.UtcNow), null);
        ctx.Characters.Add(character);

        var currency = new Currency(Guid.NewGuid(), "Gold", null);
        ctx.Currencies.Add(currency);

        var artifact = new Artifact(Guid.NewGuid(), "Sword", "A sharp blade", null);
        ctx.Artifacts.Add(artifact);

        var balance = new Balance(Guid.NewGuid(), character, currency, 1000m, null);
        ctx.Balances.Add(balance);

        var storeTransactionStatusRepo = new StoreTransactionStatusRepository(ctx);
        var storeTransactionTypeRepo = new StoreTransactionTypeRepository(ctx);
        
        var transactionStatusEnumeration = await storeTransactionStatusRepo.FindAsync(
            new StoreTransactionStatusFilter("success"));
        var transactionStatus = transactionStatusEnumeration.First();

        var transactionTypeEnumeration = await storeTransactionTypeRepo.FindAsync(
            new StoreTransactionTypeFilter("purchase"));
        var transactionType = transactionTypeEnumeration.First();

        var transaction = new StoreTransaction(
            Guid.NewGuid(), 
            balance, artifact, 150m,
            transactionType,
            transactionStatus,
            DateTime.UtcNow);
        ctx.StoreTransactions.Add(transaction);

        await ctx.SaveChangesAsync();

        await using var verifyCtx = fixture.CreateContext();
        var result = await new StoreTransactionRepository(verifyCtx).GetByIdAsync(transaction.Id);

        Assert.NotNull(result);
        Assert.Equal(transaction.Id, result.Id);
        Assert.Equal(150m, result.Amount);
        Assert.Equal("purchase", result.StoreTransactionType.Name);
        Assert.Equal("success", result.StoreTransactionStatus.Name);
        Assert.NotNull(result.Artifact);
        Assert.NotNull(result.Balance);
    }
}