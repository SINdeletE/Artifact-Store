using ArtifactStore.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Common.Models.Filters;

public class UserCharacterFilter : BaseEntityFilter<Character>
{
    Guid _userId;

    public UserCharacterFilter(Guid userId)
    {
        _userId = userId;
    }

    public override IQueryable<Character> Filter(IQueryable<Character> entities)
    {
        return entities
            .Where(c => c.UserId == _userId);
    }
}