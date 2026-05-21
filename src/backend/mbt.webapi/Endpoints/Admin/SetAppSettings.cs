using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.BuiltIn;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using AppConfig = mbt.webapi.Domain.Entities.AppConfig;

namespace mbt.webapi.Endpoints.Admin;

[Authorize(Roles = AppRoles.AdminPolicy)]
public class SetAppSettings : EndpointBaseAsync.WithRequest<AppConfigDto>.WithActionResult<AppConfigDto>
{
    private readonly IApiService _apiService;

    public SetAppSettings(IApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpPost("api/admin/settings")]
    [SwaggerOperation(
        Summary = "Set app settings",
        Description = "Set app settings",
        OperationId = "Admin.SetAppSettings",
        Tags = new[] { "Admin" })]
    public override async Task<ActionResult<AppConfigDto>> HandleAsync([FromBody] AppConfigDto request,
        CancellationToken cancellationToken = new())
    {
        var config = new AppConfig()
        {
            ClientHostUrl = request.ClientHostUrl
        };
        await _apiService.SetAppConfigAsync(config);

        return Ok(config);
    }
}
