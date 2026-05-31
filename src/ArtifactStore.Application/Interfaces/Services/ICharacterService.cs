using ArtifactStore.Application.Models.Requires;
using ArtifactStore.Application.Models.Requires.Character;
using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Interfaces.Services;

public interface ICharacterService
{
    public Task<Character> AddCharacter(AddCharacterRequire req);
    public Task DeleteCharacter(DeleteCharacterRequire req);
    public Task<PageResult<Character>> GetCharacterPage(GetCharacterPageRequire req);
    public Task<PageResult<Character>> GetUserCharacterPage(GetUserCharacterPageRequire req);
    public Task<IEnumerable<Character>> GetUserCharacters(Guid accountId);
    public Task<Character> GetCharacterById(GetCharacterByIdRequire req);
}