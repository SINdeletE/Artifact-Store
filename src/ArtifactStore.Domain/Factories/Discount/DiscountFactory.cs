namespace ArtifactStore.Domain.Factories.Discount;

public class DiscountFactory : IDiscountFactory
{
    public Entities.Discount Create(string name, string description, decimal percent, DateTimeOffset startsAt, DateTimeOffset endsAt,
        DateTimeOffset? deletedAt)
    {
        return new Entities.Discount(Guid.NewGuid(), name, description, percent, startsAt, endsAt, deletedAt);
    }
}