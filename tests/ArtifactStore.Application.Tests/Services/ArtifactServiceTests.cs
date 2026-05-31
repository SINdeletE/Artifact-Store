using ArtifactStore.Application.Models.Requires.Artifact;
using ArtifactStore.Application.Services;
using ArtifactStore.Application.Interfaces.Repositories;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using ArtifactStore.Domain.Factories.Artifact;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ArtifactStore.Application.Tests.Services;

public class ArtifactServiceTests
{
    private readonly IArtifactRepository _repo = Substitute.For<IArtifactRepository>();
    private readonly IArtifactFactory _factory = Substitute.For<IArtifactFactory>();
    private readonly ArtifactService _sut;

    public ArtifactServiceTests() => _sut = new ArtifactService(_repo, _factory, NullLogger<ArtifactService>.Instance);

    [Fact]
    public async Task AddArtifact_CallsFactoryAndRepo_ReturnsArtifact()
    {
        var artifact = new Artifact(Guid.NewGuid(), "Sword", "A sword.", null);
        _factory.Create("Sword", "A sword.", null).Returns(artifact);
        _repo.InsertAsync(artifact).Returns(artifact);

        var result = await _sut.AddArtifact(new AddArtifactRequire("Sword", "A sword."));

        _factory.Received().Create("Sword", "A sword.", null);
        await _repo.Received().InsertAsync(artifact);
        Assert.Equal(artifact, result);
    }

    [Fact]
    public async Task GetArtifactById_ReturnsArtifact_WhenFound()
    {
        var id = Guid.NewGuid();
        var artifact = new Artifact(Guid.NewGuid(), "Ring", "Magic ring.", null);
        _repo.GetByIdAsync(id).Returns(artifact);

        var result = await _sut.GetArtifactById(new GetArtifactByIdRequire { Id = id });

        Assert.Equal(artifact, result);
    }

    [Fact]
    public async Task GetArtifactById_ThrowsNotFoundException_WhenNotFound()
    {
        _repo.GetByIdAsync(Arg.Any<Guid>()).ThrowsAsync(new NotFoundException("Artifact not found"));

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _sut.GetArtifactById(new GetArtifactByIdRequire { Id = Guid.NewGuid() }));
    }

    [Fact]
    public async Task UpdateArtifact_ThrowsNotFoundException_WhenRepoThrows()
    {
        var id = Guid.NewGuid();
        var artifact = new Artifact(Guid.NewGuid(), "Axe", "Battle axe.", null);
        _repo.GetByIdAsync(id).Returns(artifact);
        _repo.UpdateAsync(artifact).ThrowsAsync(new NotFoundException("Artifact not found"));

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _sut.UpdateArtifact(new UpdateArtifactRequire { Id = id, Name = "Axe+", Description = "Upgraded." }));
    }
}
