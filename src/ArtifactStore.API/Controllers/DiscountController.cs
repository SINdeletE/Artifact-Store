using ArtifactStore.Application.Interfaces.Services;
using ArtifactStore.Application.Models.Requires.Discount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtifactStore.WebAPI.Controllers;

[ApiController]
[Route("api/discount")]
[Authorize(Roles = "admin")]
public class DiscountController : ControllerBase
{
    IDiscountService _discountService;
    
    public DiscountController(IDiscountService discountService)
    {
        _discountService = discountService;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddDiscount([FromBody] AddDiscountRequire req)
    {
        var discount = await _discountService.AddDiscount(req);
        return Ok(discount);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateDiscount([FromBody] UpdateDiscountRequire req)
    {
        await _discountService.UpdateDiscount(req);
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteDiscount(Guid id)
    {
        await _discountService.RemoveDiscount(new RemoveDiscountRequire
        {
            Id = id
        });
        return Ok();
    }
}