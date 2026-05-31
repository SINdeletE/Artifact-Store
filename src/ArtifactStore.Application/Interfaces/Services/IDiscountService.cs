using ArtifactStore.Application.Models.Requires.Discount;
using ArtifactStore.Domain.Entities;

namespace ArtifactStore.Application.Interfaces.Services;

public interface IDiscountService
{
    public Task<Discount> AddDiscount(AddDiscountRequire req);
    public Task RemoveDiscount(RemoveDiscountRequire req);
    public Task UpdateDiscount(UpdateDiscountRequire req);
}