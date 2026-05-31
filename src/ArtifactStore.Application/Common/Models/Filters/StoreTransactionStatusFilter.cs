using ArtifactStore.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Common.Models.Filters;

public class StoreTransactionStatusFilter : BaseEntityFilter<StoreTransactionStatus>
{
    private readonly string _name;

    public StoreTransactionStatusFilter(string name)
    {
        _name = name;
    }

    public override IQueryable<StoreTransactionStatus> Filter(IQueryable<StoreTransactionStatus> entities)
    {
        return base.Filter(entities)
            .Where(b => b.Name == _name);
    }
}