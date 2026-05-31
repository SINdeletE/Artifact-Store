using ArtifactStore.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Common.Models.Filters;

public class StoreTransactionTypeFilter : BaseEntityFilter<StoreTransactionType>
{
    private readonly string _name;

    public StoreTransactionTypeFilter(string name)
    {
        _name = name;
    }

    public override IQueryable<StoreTransactionType> Filter(IQueryable<StoreTransactionType> entities)
    {
        return base.Filter(entities)
            .Where(b => b.Name == _name);
    }
}