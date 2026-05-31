using ArtifactStore.Auth.Application.Interfaces.Repositories.Base;

namespace ArtifactStore.Auth.Application.Interfaces.Repositories.Account;

public interface IAccountRepository : IReadRepository<Auth.Domain.Entities.Account>, 
    IWriteRepository<Auth.Domain.Entities.Account>, IFilterRepository<Auth.Domain.Entities.Account>
{
    
}