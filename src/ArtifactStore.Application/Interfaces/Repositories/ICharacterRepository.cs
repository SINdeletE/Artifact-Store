using ArtifactStore.Application.Interfaces.Repositories.Base;
using ArtifactStore.Application.Models.Requires.Character;
using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Interfaces.Repositories;

public interface ICharacterRepository : IReadRepository<Character>, 
    IWriteRepository<Character>, IFilterRepository<Character>
{
    // public Task<IEnumerable<Character>> GetCharacterPage(int page, int pageSize);
    // public Task<Character?> GetCharacterById(Guid id);
    // public Task AddCharacter(Character character);
    // public Task<bool> DeleteCharacter(Guid id);
}