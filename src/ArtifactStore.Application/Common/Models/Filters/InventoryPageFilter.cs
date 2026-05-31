using ArtifactStore.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Common.Models.Filters;

public class InventoryPageFilter : BaseEntityFilter<InventoryItem>
{
    private readonly Guid _characterId;
    private readonly int _page;
    private readonly int _pageSize;

    public InventoryPageFilter(Guid characterId, int page, int pageSize)
    {
        _characterId = characterId;
        _page = page;
        _pageSize = pageSize;
    }

    public override IQueryable<InventoryItem> Filter(IQueryable<InventoryItem> entities)
    {
        return entities
            .Where(b => b.Character.Id == _characterId)
            .Skip((_page - 1) * _pageSize)
            .Take(_pageSize);
    }
}
