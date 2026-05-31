namespace ArtifactStore.WebApp.Shared.Models.Discount;

public class Discount
{
    public Guid Id { get; private set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Percent { get; set; }
    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public uint Version { get; set; }
}