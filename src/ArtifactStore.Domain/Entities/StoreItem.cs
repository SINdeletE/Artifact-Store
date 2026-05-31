using ArtifactStore.Domain.Common;

namespace ArtifactStore.Domain.Entities;

public class StoreItem : IEntity
{
    public Guid Id { get; private set; }
    
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

    public Guid ArtifactId { get; set; }

    public Artifact Artifact
    {
        get;
        set
        {
            field = value;
            ArtifactId = value.Id;
        }
    }

    public decimal Price { get; set; }
    public uint Version { get; set; }
    
    public Guid? DiscountId { get; set; }

    public Discount? Discount
    {
        get;
        set
        {
            field = value;
            DiscountId = value?.Id;
        }
    }

    public StoreItem() {}
    public StoreItem(Guid id, Currency currency, Artifact artifact, decimal price)
    {
        Id = id;
        Currency = currency;
        Artifact = artifact;
        Price = price;
    }
    public StoreItem(Guid id, Currency currency, Artifact artifact, decimal price, Discount discount)
    {
        Id = id;
        Currency = currency;
        Artifact = artifact;
        Price = price;
        Discount = discount;
    }
}