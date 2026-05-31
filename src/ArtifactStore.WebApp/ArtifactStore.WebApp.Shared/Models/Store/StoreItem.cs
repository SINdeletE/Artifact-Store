using ArtifactStore.WebApp.Shared.Models.Artifact;
using ArtifactStore.WebApp.Shared.Models.Currency;

namespace ArtifactStore.WebApp.Shared.Models.Store;

public sealed class StoreItem
{
    public Guid Id { get; set; }
    public Guid CurrencyId { get; set; }
    public ShopCurrency? Currency { get; set; }
    public Guid ArtifactId { get; set; }
    public ShopArtifact? Artifact { get; set; }
    public decimal Price { get; set; }
    public uint Version { get; set; }
    
    public Guid? DiscountId { get; set; }
    public Discount.Discount? Discount { get; set; }
}
