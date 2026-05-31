using ArtifactStore.Application.Models.Requires.Balance;
using ArtifactStore.Application.Models.Requires.Character;
using ArtifactStore.Application.Models.Requires.Currency;
using ArtifactStore.Application.Services;
using ArtifactStore.Application.Interfaces.Repositories;
using ArtifactStore.Application.Interfaces.Services;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using ArtifactStore.Domain.Factories.Balance;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ArtifactStore.Application.Tests.Services;

public class BalanceServiceTests
{
    private readonly IBalanceRepository _repo = Substitute.For<IBalanceRepository>();
    private readonly ICharacterService _characterService = Substitute.For<ICharacterService>();
    private readonly ICurrencyService _currencyService = Substitute.For<ICurrencyService>();
    private readonly IBalanceFactory _factory = Substitute.For<IBalanceFactory>();
    private readonly BalanceService _sut;
    
    private readonly IDistributedCache _cache = Substitute.For<IDistributedCache>();

    public BalanceServiceTests()
    {
        _cache
            .GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<byte[]?>(null));
        _cache.RemoveAsync(Arg.Any<string>()).Returns(Task.CompletedTask);
        _cache.SetAsync(Arg.Any<string>(), Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>()).Returns(Task.CompletedTask);
        
        _sut = new BalanceService(_repo, _factory, _characterService, _currencyService,
            NullLogger<BalanceService>.Instance, _cache);
    }

    private static Character MakeCharacter(Guid? userId = null) =>
        new(Guid.NewGuid(), userId ?? Guid.NewGuid(), "Hero", DateOnly.FromDateTime(DateTime.Today), null);

    private static Currency MakeCurrency() =>
        new(Guid.NewGuid(), "Gold", null);

    [Fact]
    public async Task AddBalanceWithCurrency_CallsFactoryAndRepo()
    {
        var accountId = Guid.NewGuid();
        var character = MakeCharacter(userId: accountId);
        var currency = MakeCurrency();
        var balance = new Balance(Guid.NewGuid(), character, currency, 100m, null);

        _characterService.GetCharacterById(Arg.Any<GetCharacterByIdRequire>()).Returns(character);
        _currencyService.GetCurrencyById(Arg.Any<GetCurrencyByIdRequire>()).Returns(currency);
        _factory.Create(character, currency, 100m, null).Returns(balance);
        _repo.InsertAsync(balance).Returns(balance);

        var result = await _sut.AddBalanceWithCurrency(new AddBalanceWithCurrencyRequire
        {
            CharacterId = character.Id,
            CurrencyId = currency.Id,
            Amount = 100m,
            AccountId = accountId,
            IsPrivileged = false
        });

        _factory.Received().Create(character, currency, 100m, null);
        await _repo.Received().InsertAsync(balance);
        Assert.Equal(balance, result);
    }

    [Fact]
    public async Task AddBalanceWithCurrency_ThrowsForbidden_WhenNotOwnerAndNotPrivileged()
    {
        var character = MakeCharacter();
        var currency = MakeCurrency();

        _characterService.GetCharacterById(Arg.Any<GetCharacterByIdRequire>()).Returns(character);
        _currencyService.GetCurrencyById(Arg.Any<GetCurrencyByIdRequire>()).Returns(currency);

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _sut.AddBalanceWithCurrency(new AddBalanceWithCurrencyRequire
            {
                CharacterId = character.Id,
                CurrencyId = currency.Id,
                Amount = 100m,
                AccountId = Guid.NewGuid(),
                IsPrivileged = false
            }));
    }

    [Fact]
    public async Task AddBalanceWithCurrency_Succeeds_WhenPrivileged()
    {
        var character = MakeCharacter();
        var currency = MakeCurrency();
        var balance = new Balance(Guid.NewGuid(), character, currency, 100m, null);

        _characterService.GetCharacterById(Arg.Any<GetCharacterByIdRequire>()).Returns(character);
        _currencyService.GetCurrencyById(Arg.Any<GetCurrencyByIdRequire>()).Returns(currency);
        _factory.Create(character, currency, 100m, null).Returns(balance);
        _repo.InsertAsync(balance).Returns(balance);

        var result = await _sut.AddBalanceWithCurrency(new AddBalanceWithCurrencyRequire
        {
            CharacterId = character.Id,
            CurrencyId = currency.Id,
            Amount = 100m,
            AccountId = Guid.NewGuid(),
            IsPrivileged = true
        });

        Assert.Equal(balance, result);
    }

    [Fact]
    public async Task UpdateBalance_SetsPropertiesAndCallsRepo()
    {
        var character = MakeCharacter();
        var currency = MakeCurrency();
        var balanceId = Guid.NewGuid();
        var balance = new Balance(balanceId, character, currency, 50m, null);
        _repo.GetByIdAsync(balanceId).Returns(balance);

        await _sut.UpdateBalance(new UpdateBalanceRequire
        {
            Id = balanceId,
            Character = character,
            Currency = currency,
            Amount = 200m,
            Version = 1
        });

        Assert.Equal(200m, balance.Amount);
        await _repo.Received().UpdateAsync(balance);
    }

    [Fact]
    public async Task UpdateBalance_ThrowsNotFoundException_WhenBalanceNotFound()
    {
        var balanceId = Guid.NewGuid();
        _repo.GetByIdAsync(balanceId).ThrowsAsync(new NotFoundException("Balance not found"));

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _sut.UpdateBalance(new UpdateBalanceRequire
            {
                Id = balanceId,
                Character = MakeCharacter(),
                Currency = MakeCurrency(),
                Amount = 200m
            }));
    }
}
