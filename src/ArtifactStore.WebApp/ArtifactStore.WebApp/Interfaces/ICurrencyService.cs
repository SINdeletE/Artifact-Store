using ArtifactStore.WebApp.Responses;

namespace ArtifactStore.WebApp.Interfaces;

public interface ICurrencyService
{
    public Task<CurrencyResponse> GetCurrencyById(Guid id);
}
