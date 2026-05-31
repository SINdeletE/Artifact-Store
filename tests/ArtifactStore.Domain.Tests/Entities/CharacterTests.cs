using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Domain.Tests.Entities;

public class CharacterTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var userId = Guid.NewGuid();
        var date = new DateOnly(2023, 6, 1);

        var character = new Character(Guid.NewGuid(), userId, "Gandalf", date, null);

        Assert.Equal(userId, character.UserId);
        Assert.Equal("Gandalf", character.Name);
        Assert.Equal(date, character.CreationDate);
        Assert.NotEqual(Guid.Empty, character.Id);
    }


}
