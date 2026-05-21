using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.BuiltIn;
using mbt.webapi.Services.Interfaces;
using mbt.webapi.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.CurrencyRates;

[Authorize(Roles = AppRoles.AdminPolicy)]
public class UploadExchangeRates : EndpointBaseAsync.WithRequest<FileModel>.WithActionResult
{
    private readonly ICurrencyService _currencyService;

    public UploadExchangeRates(ICurrencyService currencyService)
    {
        _currencyService = currencyService;
    }

    [HttpPost("api/currencyRates/upload")]
    [SwaggerOperation(
        Summary = "Upload currency rates CSV file",
        Description = "Upload currency rates CSV file",
        OperationId = "CurrencyRates.Upload",
        Tags = new[] { "CurrencyRates" })]
    public override async Task<ActionResult> HandleAsync([FromForm] FileModel formData,
        CancellationToken cancellationToken = new())
    {
        await _currencyService.ImportCsvExchangeRates(formData);

        return Ok();
    }
}
