namespace ArtifactStore.Domain.Factories.Discount;

public interface IDiscountFactory
{
    public Entities.Discount Create(string name, string description, decimal percent, DateTimeOffset startsAt, DateTimeOffset endsAt, DateTimeOffset? deletedAt);
}