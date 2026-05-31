using ArtifactStore.Auth.Application.Interfaces.Repositories.Account;
using ArtifactStore.Auth.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Auth.Domain.Exceptions;
using ArtifactStore.Auth.Infrastructure.Context;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ArtifactStore.Auth.Infrastructure.Repositories.Mongo;

public class MongoAccountRepository : IAccountRepository
{
    IMongoDbContext _dbContext;
    IMongoDbContext _adminDbContext;

    public MongoAccountRepository(IMongoDbContext dbContext,
        [FromKeyedServices("Admin")] IMongoDbContext adminDbContext)
    {
        _dbContext = dbContext;
        _adminDbContext = adminDbContext;
    }

    public async Task<Auth.Domain.Entities.Account> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _dbContext.Accounts
                .Find(a => a.Id == id)
                .FirstOrDefaultAsync();
            if (entity is null)
                throw new NotFoundException("Account not found");

            await PopulateRolesAsync(new[] { entity });
            return entity;
        }
        catch (NotFoundException)
        {
            throw new NotFoundException("Account not found");
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

    private async Task PopulateRolesAsync(IReadOnlyCollection<Auth.Domain.Entities.Account> accounts)
    {
        if (accounts.Count == 0) return;

        var roleIds = accounts.Select(a => a.AccountRoleId).Distinct().ToList();
        var roles = await _dbContext.AccountRoles
            .Find(r => roleIds.Contains(r.Id))
            .ToListAsync();
        var byId = roles.ToDictionary(r => r.Id);

        foreach (var a in accounts)
            if (byId.TryGetValue(a.AccountRoleId, out var role))
                a.AccountRole = role;
    }

    public async Task DeleteAsync(Auth.Domain.Entities.Account entity)
    {
        UpdateResult result;

        try
        {
            var update = Builders<Auth.Domain.Entities.Account>.Update
                .Set(a => a.DeletedAt, DateTimeOffset.UtcNow);
            result = await _dbContext.Accounts.UpdateOneAsync(a => a.Id == entity.Id, update);
        }
        catch (MongoConnectionException e)
        {
            throw new RetryConnectionException(e.Message);
        }
        catch (Exception e)
        {
            throw new ServerException(e.Message);
        }

        if (result.MatchedCount == 0)
        {
            throw new NotFoundException("Account not found");
        }
    }

    public async Task<Auth.Domain.Entities.Account> InsertAsync(Auth.Domain.Entities.Account entity)
    {
        try
        {
            var role = await _adminDbContext.AccountRoles
                .Find(r => r.Name.ToLower() == entity.AccountRole.Name.ToLower())
                .FirstOrDefaultAsync();
            if (role is null)
                throw new NotFoundException("Role not found");

            if (entity.Password.Length < 8)
                throw new BadRequestException("Password is too short");

            var nicknameExists = await _adminDbContext.Accounts
                .Find(a => a.Nickname == entity.Nickname).AnyAsync();
            if (nicknameExists)
                throw new ConflictException("Nickname already exists");

            var emailExists = await _adminDbContext.Accounts
                .Find(a => a.Email == entity.Email).AnyAsync();
            if (emailExists)
                throw new ConflictException("Email already exists");

            entity.AccountRole = role;

            await _adminDbContext.Accounts.InsertOneAsync(entity);
            return entity;
        }
        catch (NotFoundException)
        {
            throw new NotFoundException("Account not found");
        }
        catch (ConflictException)
        {
            throw new ConflictException("Account already exists");
        }
        catch (BadRequestException)
        {
            throw new BadRequestException("Bad Request");
        }
        catch (MongoConnectionException e)
        {
            throw new RetryConnectionException(e.Message);
        }
        catch (Exception e)
        {
            throw new InternalServerErrorException(e.Message);
        }
    }

    public async Task UpdateAsync(Auth.Domain.Entities.Account entity)
    {
        ReplaceOneResult result;

        var currentVersion = entity.Version;
        var filter = Builders<Auth.Domain.Entities.Account>.Filter.And(
            Builders<Auth.Domain.Entities.Account>.Filter.Eq(a => a.Id, entity.Id),
            Builders<Auth.Domain.Entities.Account>.Filter.Eq(a => a.Version, currentVersion));
        entity.Version = currentVersion + 1;

        try
        {
            result = await _dbContext.Accounts.ReplaceOneAsync(filter, entity);
        }
        catch (MongoConnectionException e)
        {
            throw new RetryConnectionException(e.Message);
        }
        catch (Exception e)
        {
            throw new ServerException(e.Message);
        }

        if (result.MatchedCount == 0)
        {
            var exists = await _dbContext.Accounts
                .Find(a => a.Id == entity.Id).AnyAsync();
            if (exists)
                throw new ConflictException("Concurrency conflict");
            throw new NotFoundException("Account not found");
        }
    }

    public async Task<IEnumerable<Auth.Domain.Entities.Account>> FindAsync(BaseEntityFilter<Auth.Domain.Entities.Account> filter)
    {
        try
        {
            var accounts = await MongoQueryable.ToListAsync(
                filter.Filter(_dbContext.Accounts.AsQueryable()));
            await PopulateRolesAsync(accounts);
            return accounts;
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
