using ArtifactStore.Application.Common.Models.Filters;
using ArtifactStore.Application.Models.Requires.Character;
using ArtifactStore.Application.Services;
using ArtifactStore.Application.Interfaces.Repositories;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using ArtifactStore.Domain.Factories.Character;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ArtifactStore.Application.Tests.Services;

public class CharacterServiceTests
{
    private readonly ICharacterRepository _repo = Substitute.For<ICharacterRepository>();
    private readonly ICharacterFactory _factory = Substitute.For<ICharacterFactory>();
    private readonly CharacterService _sut;
    
    private readonly IDistributedCache _cache = Substitute.For<IDistributedCache>();

    public CharacterServiceTests()
    {
        _cache
            .GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<byte[]?>(null));
        _cache.RemoveAsync(Arg.Any<string>()).Returns(Task.CompletedTask);
        _cache.SetAsync(Arg.Any<string>(), Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>()).Returns(Task.CompletedTask);
        
        _sut = new CharacterService(_repo, _factory, NullLogger<CharacterService>.Instance, _cache);
    }

    private static Character MakeCharacter() =>
        new(Guid.NewGuid(), Guid.NewGuid(), "Hero", DateOnly.FromDateTime(DateTime.Today), null);

    [Fact]
    public async Task AddCharacter_CallsFactoryAndRepo_ThenGetById_ReturnsCharacter()
    {
        var character = MakeCharacter();
        var req = new AddCharacterRequire { UserId = character.UserId, Name = character.Name };
        _factory.Create(character.UserId, character.Name, null).Returns(character);
        _repo.InsertAsync(character).Returns(character);
        _repo.GetByIdAsync(character.Id).Returns(character);

        await _sut.AddCharacter(req);
        var result = await _sut.GetCharacterById(new GetCharacterByIdRequire { Id = character.Id });

        await _repo.Received().InsertAsync(character);
        Assert.Equal(character, result);
    }

    [Fact]
    public async Task DeleteCharacter_ThrowsNotFoundException_WhenCharacterNotFound()
    {
        _repo.GetByIdAsync(Arg.Any<Guid>()).ThrowsAsync(new NotFoundException("Character not found"));

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _sut.DeleteCharacter(new DeleteCharacterRequire { Id = Guid.NewGuid() }));
    }

    [Fact]
    public async Task GetCharacterPage_ReturnsPageResult_WithCorrectCount()
    {
        var characters = new List<Character> { MakeCharacter(), MakeCharacter() };
        _repo.FindAsync(Arg.Any<PageEntityFilter<Character>>()).Returns(characters.AsEnumerable());

        var result = await _sut.GetCharacterPage(new GetCharacterPageRequire { Page = 1, PageSize = 10 });

        Assert.Equal(2, result.TotalItems);
        Assert.Equal(2, result.Items.Count());
    }
}
