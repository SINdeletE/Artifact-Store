using ArtifactStore.Application.Interfaces.Repositories.Filters;
using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Common.Models.Filters;

public class BalanceCharacterCurrencyFilter : BaseEntityFilter<Balance>
{
    private readonly Guid _characterId;
    private readonly Guid _currencyId;

    public BalanceCharacterCurrencyFilter(Guid characterId, Guid currencyId)
    {
        _characterId = characterId;
        _currencyId = currencyId;
    }

    public override IQueryable<Balance> Filter(IQueryable<Balance> entities)
    {
        return entities
            .Where(b => b.Currency.Id == _currencyId)
            .Where(b => b.Character.Id == _characterId);
    }
}