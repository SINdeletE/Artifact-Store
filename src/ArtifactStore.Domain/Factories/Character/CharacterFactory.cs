namespace ArtifactStore.Domain.Factories.Character;

public class CharacterFactory : ICharacterFactory
{
    public Entities.Character Create(Guid userId, string name, DateTimeOffset? deletedAt)
    {
        var createDate = DateOnly.FromDateTime(DateTime.UtcNow);
        
        return new Entities.Character(Guid.NewGuid(), userId, name, createDate, deletedAt);
    }
}