using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.CurrencyRates;

public class GetCurrencyRates : EndpointBaseAsync.WithoutRequest.WithActionResult<List<CurrencyRateDto>>
{
    private readonly ICurrencyService _currencyService;

    public GetCurrencyRates(ICurrencyService currencyService)
    {
        _currencyService = currencyService;
    }

    [HttpGet("api/currencyRates")]
    [SwaggerOperation(
        Summary = "List currency rates",
        Description = "List currency rates",
        OperationId = "CurrencyRates.List",
        Tags = new[] { "CurrencyRates" })]
    public override async Task<ActionResult<List<CurrencyRateDto>>> HandleAsync(
        CancellationToken cancellationToken = new())
    {
        var currencyRates = await _currencyService.GetCurrencyRates();

        var currencyRatesDtoList = currencyRates.Select(CurrencyRateDto.FromCurrencyRate);

        return Ok(currencyRatesDtoList);
    }
}
