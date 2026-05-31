using ArtifactStore.Application.Interfaces.Repositories;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Exceptions;
using Microsoft.EntityFrameworkCore.Storage;

namespace ArtifactStore.Infrastructure.Repositories;

public class CurrencyRepository : ICurrencyRepository
{
    IApplicationContext _dbContext;
    
    public CurrencyRepository(IApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Currency> GetByIdAsync(Guid id)
    {
        try
        {
            return await _dbContext.Currencies
                .Where(c => c.Id == id)
                .FirstAsync();
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException("Currency not found");
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