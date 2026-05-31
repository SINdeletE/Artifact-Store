using ArtifactStore.Auth.Application.Interfaces.Repositories.Account;
using ArtifactStore.Auth.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Auth.Domain.Entities;
using ArtifactStore.Auth.Domain.Exceptions;
using Microsoft.EntityFrameworkCore.Storage;

namespace ArtifactStore.Auth.Infrastructure.Repositories.Account;

public class AccountRoleRepository : IAccountRoleRepository
{
    IApplicationContext _dbContext;

    public AccountRoleRepository(IApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<AccountRole>> FindAsync(BaseEntityFilter<AccountRole> filter)
    {
        try
        {
            return await filter
                .Filter(_dbContext.AccountRoles)
                .ToListAsync();
        }
        catch (RetryLimitExceededException e)
        {
            throw new RetryConnectionException(e.Message);
        }
        catch (Exception e)
        {
            throw new ServerException(e.Message);
        }
    }
}