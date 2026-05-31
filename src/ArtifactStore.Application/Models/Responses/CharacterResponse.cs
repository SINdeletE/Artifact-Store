using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Models.Responses;

public class CharacterResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public DateOnly CreationDate { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public uint Version { get; set; }
    
    public CharacterResponse(){}

    public CharacterResponse(Character character)
    {
        Id = character.Id;
        UserId = character.UserId;
        Name = character.Name;
        CreationDate = character.CreationDate;
        DeletedAt = character.DeletedAt;
        Version = character.Version;
    }

    public Character Get()
    {
        return new Character(Id, UserId, Name, CreationDate, DeletedAt)
        {
            Version = Version
        };
    }
}