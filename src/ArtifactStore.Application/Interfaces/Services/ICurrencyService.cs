using ArtifactStore.Application.Models.Requires.Currency;
using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Interfaces.Services;

public interface ICurrencyService
{
    public Task<Currency> GetCurrencyById(GetCurrencyByIdRequire req);
}