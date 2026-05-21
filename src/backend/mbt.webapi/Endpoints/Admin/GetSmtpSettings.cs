using Ardalis.ApiEndpoints;
using mbt.webapi.BuiltIn;
using mbt.webapi.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.Admin;

[Authorize(Roles = AppRoles.AdminPolicy)]
public class GetSmtpSettings : EndpointBaseSync.WithoutRequest.WithActionResult<ISmtpSettings>
{
    private readonly ISmtpSettings _smtpSettings;


    public GetSmtpSettings(ISmtpSettings smtpSettings)
    {
        _smtpSettings = smtpSettings;
    }

    [HttpGet("api/admin/smtpSettings")]
    [SwaggerOperation(
        Summary = "Get smtp settings",
        Description = "Get smtp settings",
        OperationId = "Admin.GetSmtpSettings",
        Tags = new[] { "Admin" })]
    public override ActionResult<ISmtpSettings> Handle()
    {
        var config = _smtpSettings;

        return Ok(config);
    }
}
