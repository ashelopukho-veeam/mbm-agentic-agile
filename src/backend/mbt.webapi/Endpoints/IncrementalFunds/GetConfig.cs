using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.BuiltIn;
using mbt.webapi.Endpoints.IncrementalFunds.dto;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.IncrementalFunds;

public class GetConfig : EndpointBaseAsync.WithoutRequest.WithResult<IncrementalFundsConfigDto>
{
    private readonly IIncrementalFundsService _incrementalFundsService;

    public GetConfig(IIncrementalFundsService incrementalFundsService)
    {
        _incrementalFundsService = incrementalFundsService;
    }

    [HttpGet("api/incrementalFunds/config")]
    [Authorize(Roles = AppRoles.ViewPolicy)]
    [SwaggerOperation(
        Summary = "Get incremental funds config",
        Description = "Get incremental funds config",
        OperationId = "IncrementalFunds.GetConfig",
        Tags = new[] { "IncrementalFunds" })]
    public override async Task<IncrementalFundsConfigDto> HandleAsync(
        CancellationToken cancellationToken = new())
    {
        var config = await _incrementalFundsService.GetConfig();

        var dto =
            new IncrementalFundsConfigDto
            {
                WorkflowApproverId = config?.WorkflowApproverId ?? ""
            };

        return dto;
    }
}
