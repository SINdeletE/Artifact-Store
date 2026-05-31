using ArtifactStore.Auth.Application.Interfaces.Repositories.Account;
using ArtifactStore.Auth.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Auth.Domain.Exceptions;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;

namespace ArtifactStore.Auth.Infrastructure.Repositories.Account;

public class AccountRepostiory : IAccountRepository
{
    IApplicationContext _dbContext;
    
    public AccountRepostiory(IApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Auth.Domain.Entities.Account> GetByIdAsync(Guid id)
    {
        try
        {
            return await _dbContext.Accounts
                .Where(a => a.Id == id)
                .FirstAsync();
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException("Account not found");
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

    public async Task DeleteAsync(Auth.Domain.Entities.Account entity)
    {
        int rows = 0;

        try
        {
            rows = await _dbContext.Accounts
                .Where(a => a.Id == entity.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(a => a.DeletedAt, DateTimeOffset.UtcNow));
        }
        catch (RetryLimitExceededException e)
        {
            throw new RetryConnectionException(e.Message);
        }
        catch (Exception e)
        {
            throw new ServerException(e.Message);
        }

        if (rows == 0)
        {
            throw new NotFoundException("Account not found");
        }
    }

    public async Task<Auth.Domain.Entities.Account> InsertAsync(Auth.Domain.Entities.Account entity)
    {
        try
        {
            await _dbContext.Database.ExecuteSqlRawAsync(
                "CALL register_account(@id, @role_name, @nickname," +
                "@password, @email, @register_date)",
                new NpgsqlParameter("@id", entity.Id),
                new NpgsqlParameter("@role_name", entity.AccountRole.Name),
                new NpgsqlParameter("@nickname", entity.Nickname),
                new NpgsqlParameter("@password", entity.Password),
                new NpgsqlParameter("@email", entity.Email),
                new NpgsqlParameter("@register_date", entity.RegisterDate));
            
            return entity;
        }
        catch (RetryLimitExceededException e)
        {
            throw new RetryConnectionException(e.Message);
        }
        catch (PostgresException e)
        {
            throw e.MessageText switch
            {
                "ROLE NOT FOUND" => new NotFoundException("Role not found"),
                "NICKNAME ALREADY EXISTS" => new ConflictException("Nickname already exists"),
                "EMAIL ALREADY EXISTS" => new ConflictException("Email already exists"),
                "PASSWORD IS TOO SHORT" => new BadRequestException("Password is too short"),
                _ => new InternalServerErrorException("Unexpected error occured")
            };
        }
        catch (Exception e)
        {
            throw new ServerException(e.Message);
        }
    }

    public async Task UpdateAsync(Auth.Domain.Entities.Account entity)
    {
        int rows = 0;
        _dbContext.Accounts.Update(entity);

        try
        {
            rows = await _dbContext.SaveChangesAsync();
        }
        catch (RetryLimitExceededException e)
        {
            throw new RetryConnectionException(e.Message);
        }
        catch (DbUpdateConcurrencyException e)
        {
            throw new ConflictException(e.Message);
        }
        catch (DbUpdateException e)
        {
            throw new ServerException(e.Message);
        }
        catch (Exception e)
        {
            throw new ServerException(e.Message);
        }

        if (rows == 0)
        {
            throw new NotFoundException("Account not found");
        }
    }

    public async Task<IEnumerable<Auth.Domain.Entities.Account>> FindAsync(BaseEntityFilter<Auth.Domain.Entities.Account> filter)
    {
        try
        {
            return await filter
                .Filter(_dbContext.Accounts)
                .Include(c => c.AccountRole)
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