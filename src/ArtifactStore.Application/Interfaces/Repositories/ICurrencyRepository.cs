using ArtifactStore.Application.Interfaces.Repositories.Base;
using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Interfaces.Repositories;

public interface ICurrencyRepository : IReadRepository<Currency>
{
    // public Task<Currency?> GetCurrencyById(Guid id);
}