using ArtifactStore.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Common.Models.Filters;

public class BalancesCharacterFilter : BaseEntityFilter<Balance>
{
    private readonly Guid _characterId;

    public BalancesCharacterFilter(Guid characterId)
    {
        _characterId = characterId;
    }

    public override IQueryable<Balance> Filter(IQueryable<Balance> entities)
    {
        return entities
            .Where(b => b.Character.Id == _characterId);
    }
}