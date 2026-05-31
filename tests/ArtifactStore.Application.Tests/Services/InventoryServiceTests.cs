using ArtifactStore.Application.Models.Requires.Artifact;
using ArtifactStore.Application.Models.Requires.Character;
using ArtifactStore.Application.Models.Requires.Inventory;
using ArtifactStore.Application.Services;
using ArtifactStore.Application.Interfaces.Repositories;
using ArtifactStore.Application.Interfaces.Services;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using ArtifactStore.Domain.Factories.Inventory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ArtifactStore.Application.Tests.Services;

public class InventoryServiceTests
{
    private readonly IInventoryRepository _repo = Substitute.For<IInventoryRepository>();
    private readonly ICharacterService _characterService = Substitute.For<ICharacterService>();
    private readonly IArtifactService _artifactService = Substitute.For<IArtifactService>();
    private readonly IInventoryItemFactory _factory = Substitute.For<IInventoryItemFactory>();
    private readonly InventoryService _sut;
    
    private readonly IDistributedCache _cache = Substitute.For<IDistributedCache>();

    public InventoryServiceTests()
    {
        _cache
            .GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<byte[]?>(null));
        _cache.RemoveAsync(Arg.Any<string>()).Returns(Task.CompletedTask);
        _cache.SetAsync(Arg.Any<string>(), Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>()).Returns(Task.CompletedTask);
        
        _sut = new InventoryService(_repo, _characterService, _artifactService, _factory,
            NullLogger<InventoryService>.Instance, _cache);
    }

    private static Character MakeCharacter() =>
        new(Guid.NewGuid(), Guid.NewGuid(), "Hero", DateOnly.FromDateTime(DateTime.Today), null);

    [Fact]
    public async Task AddInventoryItem_CallsFactoryAndRepo_ThenGetById_ReturnsItem()
    {
        var character = MakeCharacter();
        var artifact = new Artifact(Guid.NewGuid(), "Bow", "Elven bow.", null);
        var item = new InventoryItem(Guid.NewGuid(), character, artifact);

        _characterService.GetCharacterById(Arg.Any<GetCharacterByIdRequire>()).Returns(character);
        _artifactService.GetArtifactById(Arg.Any<GetArtifactByIdRequire>()).Returns(artifact);
        _factory.Create(character, artifact).Returns(item);
        _repo.InsertAsync(item).Returns(item);
        _repo.GetByIdAsync(item.Id).Returns(item);

        await _sut.AddInventoryItem(new AddInventoryItemRequire
            { CharacterId = character.Id, ArtifactId = artifact.Id });
        var result = await _sut.GetInventoryItemById(new GetInventoryItemById { Id = item.Id });

        await _repo.Received().InsertAsync(item);
        Assert.Equal(item, result);
    }

    [Fact]
    public async Task DeleteInventoryItem_ThrowsNotFoundException_WhenItemNotFound()
    {
        _repo.GetByIdAsync(Arg.Any<Guid>()).ThrowsAsync(new NotFoundException("Inventory item not found"));

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _sut.DeleteInventoryItem(new DeleteInventoryItemRequire
                { Id = Guid.NewGuid(), AccountId = Guid.NewGuid(), IsPrivileged = false }));
    }
}
