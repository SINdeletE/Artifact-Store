using ArtifactStore.Application.Interfaces.Repositories;
using ArtifactStore.Application.Models.Requires.Discount;
using ArtifactStore.Domain.Entities;
using ArtifactStore.Domain.Factories.Discount;
using Microsoft.Extensions.Logging;

namespace ArtifactStore.Application.Services;

public class DiscountService : IDiscountService
{
    ILogger<DiscountService> _logger;
    
    IDiscountRepository _discountRepository;
    IDiscountFactory _discountFactory;

    public DiscountService(IDiscountRepository discountRepository,
        IDiscountFactory discountFactory,
        ILogger<DiscountService> logger)
    { 
        _logger = logger;
        
        _discountRepository = discountRepository;
        _discountFactory = discountFactory;
    }

    public async Task<Discount> AddDiscount(AddDiscountRequire req)
    {
        var discount = _discountFactory.Create(req.Name, req.Description,
            req.Percent, req.StartsAt, req.EndsAt, null);
        _logger.LogInformation("Created discount {DiscountId}", discount.Id);
        
        var result = await _discountRepository.InsertAsync(discount);
        _logger.LogInformation("Inserted discount {DiscountId}", discount.Id);
        
        return result;
    }

    public async Task RemoveDiscount(RemoveDiscountRequire req)
    {
        var discount = await _discountRepository.GetByIdAsync(req.Id);
        _logger.LogInformation("Fetched discount {DiscountId}", discount.Id);    
        
        await _discountRepository.DeleteAsync(discount);
        _logger.LogInformation("Deleted discount {DiscountId}", discount.Id);
    }

    public async Task UpdateDiscount(UpdateDiscountRequire req)
    {
        var discount = await _discountRepository.GetByIdAsync(req.Id);
        _logger.LogInformation("Fetched discount {DiscountId}", discount.Id);  
        
        discount.Name = req.Name;
        discount.Description = req.Description;
        discount.Percent = req.Percent;
        discount.StartsAt = req.StartsAt;
        discount.EndsAt = req.EndsAt;
        
        await _discountRepository.UpdateAsync(discount);
        _logger.LogInformation("Updated discount {DiscountId}", discount.Id);
    }
}