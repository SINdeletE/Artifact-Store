using ArtifactStore.Domain.Factories.Character;

namespace ArtifactStore.Domain.Tests.Factories;

public class CharacterFactoryTests
{
    private readonly CharacterFactory _factory = new();

    [Fact]
    public void Create_SetsPropertiesCorrectly()
    {
        var userId = Guid.NewGuid();

        var character = _factory.Create(userId, "Aragorn", null);

        Assert.Equal(userId, character.UserId);
        Assert.Equal("Aragorn", character.Name);
        Assert.NotEqual(default, character.CreationDate);
        Assert.NotEqual(Guid.Empty, character.Id);
    }


}