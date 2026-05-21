using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.BuiltIn;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.IncrementalFunds;

public class Cancel : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithoutResult
{
    private readonly IIncrementalFundsService _incrementalFundsService;
    private readonly ICurrentUserContext _currentUserContext;


    public Cancel(IIncrementalFundsService incrementalFundsService, ICurrentUserContext currentUserContext)
    {
        _incrementalFundsService = incrementalFundsService;
        _currentUserContext = currentUserContext;
    }

    [HttpPost("api/incrementalFunds/cancel")]
    [Authorize(Roles = $"{AppRoles.AdminPolicy},{AppRoles.Designers},{AppRoles.Contributors}")]
    [SwaggerOperation(
        Summary = "Cancel an Incremental Fund",
        Description = "Cancel an Incremental Fund",
        OperationId = "IncrementalFunds.Cancel",
        Tags = new[] { "IncrementalFunds" })]
    public override async Task<ActionResult> HandleAsync([FromBody] ObjectIdRequest request,
        CancellationToken cancellationToken = new())
    {

        var incrementalFund = await _incrementalFundsService.GetAsync(request.Id);
        if (incrementalFund == null)
            return NotFound();

        var isAdmin = _currentUserContext.IsInRoles(new[] { AppRoles.SysAdmins, AppRoles.Admins });
        var isAuthor = _currentUserContext.UserId == incrementalFund.CreatedBy;

        if (!isAdmin && !isAuthor)
            throw new AccessDeniedException();

        await _incrementalFundsService.CancelIncrementalFund(request.Id);

        return Ok();
    }
}
