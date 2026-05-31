using ArtifactStore.WebApp.Shared.Models.Discount;

namespace ArtifactStore.WebApp.Responses;

public class StoreItemResponse
{
    public Guid Id { get; set; }
    public Guid CurrencyId { get; set; }
    public CurrencyResponse? Currency { get; set; }
    public Guid ArtifactId { get; set; }
    public ArtifactResponse? Artifact { get; set; }
    public decimal Price { get; set; }
    public uint Version { get; set; }
    public Guid? DiscountId { get; set; }
    public Discount? Discount { get; set; }
}
