namespace ArtifactStore.Application.Models.Requires.Discount;

public class AddDiscountRequire
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Percent { get; set; }
    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }
}