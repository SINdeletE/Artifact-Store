namespace ArtifactStore.Domain.Factories.Character;

public interface ICharacterFactory
{
    public Entities.Character Create(Guid userId, string name, DateTimeOffset? deletedAt);
}