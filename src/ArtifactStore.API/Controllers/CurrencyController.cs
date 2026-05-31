using ArtifactStore.Application.Interfaces.Services;
using ArtifactStore.Application.Models.Requires.Currency;
using Microsoft.AspNetCore.Mvc;

namespace ArtifactStore.WebAPI.Controllers;

[ApiController]
[Route("api/currencies")]
public class CurrencyController : ControllerBase
{
    ICurrencyService _currencyService;
    
    public CurrencyController(ICurrencyService currencyService)
    {
        _currencyService = currencyService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCurrencyById(Guid id)
    {
        var currency = await _currencyService.GetCurrencyById(
            new GetCurrencyByIdRequire{Id = id});
        return Ok(currency);
    }
}