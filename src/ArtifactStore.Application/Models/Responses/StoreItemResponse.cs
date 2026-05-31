using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Models.Responses;

public class StoreItemResponse
{
    public Guid Id { get; set; }
    
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

    public StoreItemResponse(){}
    public StoreItemResponse(StoreItem item)
    {
        Id = item.Id;
        CurrencyId = item.CurrencyId;
        Currency = item.Currency;
        ArtifactId = item.ArtifactId;
        Artifact = item.Artifact;
        Price = item.Price;
        Version = item.Version;
        Discount = item.Discount;
    }

    public StoreItem Get()
    {
        return new StoreItem(Id, Currency, Artifact, Price)
        {
            Version = Version,
            Discount = Discount,
        };
    }
}