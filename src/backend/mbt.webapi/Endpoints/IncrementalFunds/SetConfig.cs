using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.BuiltIn;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Endpoints.IncrementalFunds.dto;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.IncrementalFunds;

public class SetConfig : EndpointBaseAsync.WithRequest<IncrementalFundsConfigDto>.WithResult<IncrementalFundsConfigDto>
{
    private readonly IUserService _userService;

    private readonly IIncrementalFundsService _incrementalFundsService;

    public SetConfig(IUserService userService, IIncrementalFundsService incrementalFundsService)
    {
        _userService = userService;
        _incrementalFundsService = incrementalFundsService;
    }

    [Authorize(Roles = AppRoles.AdminPolicy)]
    [HttpPost("api/incrementalFunds/config")]
    [SwaggerOperation(
        Summary = "Set incremental funds config",
        Description = "Set incremental funds config",
        OperationId = "IncrementalFunds.SetConfig",
        Tags = new[] { "IncrementalFunds" })]
    public override async Task<IncrementalFundsConfigDto> HandleAsync(IncrementalFundsConfigDto request,
        CancellationToken cancellationToken = new())
    {
        // validate request
        var wfUser = await _userService.GetAsync(request.WorkflowApproverId);
        if (wfUser == null)
            throw new ApiException("Workflow approver not found");

        var config = new IncrementalFundsConfig() { WorkflowApproverId = request.WorkflowApproverId };

        await _incrementalFundsService.SetConfig(config);

        return request;
    }
}
