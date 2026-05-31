using ArtifactStore.Auth.Application.Interfaces.Repositories.Account;
using ArtifactStore.Auth.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Auth.Domain.Entities;
using ArtifactStore.Auth.Domain.Exceptions;
using ArtifactStore.Auth.Infrastructure.Context;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ArtifactStore.Auth.Infrastructure.Repositories.Mongo;

public class MongoAccountRoleRepository : IAccountRoleRepository
{
    IMongoDbContext _dbContext;

    public MongoAccountRoleRepository(IMongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<AccountRole>> FindAsync(BaseEntityFilter<AccountRole> filter)
    {
        try
        {
            return await MongoQueryable.ToListAsync(
                filter.Filter(_dbContext.AccountRoles.AsQueryable()));
        }
        catch (MongoConnectionException e)
        {
            throw new RetryConnectionException(e.Message);
        }
        catch (Exception e)
        {
            throw new ServerException(e.Message);
        }
    }
}
