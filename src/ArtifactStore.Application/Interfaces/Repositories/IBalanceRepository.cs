using ArtifactStore.Application.Interfaces.Repositories.Base;
using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Interfaces.Repositories;

public interface IBalanceRepository : IReadRepository<Balance>, 
    IWriteRepository<Balance>, IFilterRepository<Balance>
{
    // public Task AddBalance(Balance balance);
    // public Task<bool> UpdateBalance(Balance balance);
    // public Task<Balance?> GetBalanceById(Guid id);
    // public Task<Balance?> GetCurrencyBalanceByCharacterId(Guid userId, Currency currency);
}