using ArtifactStore.WebApp.Requires.Character;
using ArtifactStore.WebApp.Responses;

namespace ArtifactStore.WebApp.Interfaces;

public interface ICharacterService
{
    public Task<CharacterResponse> GetCharacterById(Guid id);
    public Task<PageResponse<CharacterResponse>> GetCharacterPage(CharacterGetPageRequire req);
    public Task<PageResponse<CharacterResponse>> GetUserCharacterPage(CharacterGetUserPageRequire req);
    public Task<IEnumerable<CharacterResponse>> GetUserCharacters();
    public Task DeleteCharacter(Guid id);
    public Task<CharacterResponse> AddCharacter(CharacterAddRequire req);
}
