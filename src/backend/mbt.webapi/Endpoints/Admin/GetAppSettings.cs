using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.BuiltIn;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.Admin;

[Authorize(Roles = AppRoles.AdminPolicy)]
public class GetAppSettings : EndpointBaseAsync.WithoutRequest.WithActionResult<AppConfigDto>
{
    private readonly IApiService _apiService;

    public GetAppSettings(IApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet("api/admin/settings")]
    [SwaggerOperation(
        Summary = "Get app settings",
        Description = "Get app settings",
        OperationId = "Admin.GetAppSettings",
        Tags = new[] { "Admin" })]
    public override async Task<ActionResult<AppConfigDto>> HandleAsync(
        CancellationToken cancellationToken = new())
    {
        var config = await _apiService.GetAppConfigAsync();

        var dto = new AppConfigDto() { ClientHostUrl = config?.ClientHostUrl ?? "" };

        return Ok(dto);
    }
}
