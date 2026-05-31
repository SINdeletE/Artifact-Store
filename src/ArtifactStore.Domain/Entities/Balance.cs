using ArtifactStore.Domain.Common;

namespace ArtifactStore.Domain.Entities;

public class Balance : IEntity
{
    public Guid Id { get; private set; }
    
    public Guid CharacterId { get; set; }

    public Character Character
    {
        get;
        set
        {
            field = value;
            CharacterId = value.Id;
        }
    }

    public Guid CurrencyId { get; set; }

    public Currency Currency
    {
        get;
        set
        {
            field = value;
            CurrencyId = value.Id;
        }
    }

    public decimal Amount { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public uint Version { get; set; }

    private Balance() { }

    public Balance(Guid id, Character character, Currency currency, decimal amount, DateTimeOffset? deletedAt)
    {
        Id = id;
        Character = character;
        Currency = currency;
        Amount = amount;
        DeletedAt = deletedAt;
    }
}