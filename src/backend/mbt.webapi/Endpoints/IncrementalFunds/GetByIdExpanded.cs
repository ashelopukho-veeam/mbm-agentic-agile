using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using mbt.webapi.BuiltIn;
using mbt.webapi.Endpoints.IncrementalFunds.dto;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.IncrementalFunds;

public class
    GetByIdExpanded : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithActionResult<IncrementalFundExpandedDto>
{
    private readonly IIncrementalFundsService _incrementalFundsService;
    private readonly IMapper _mapper;

    public GetByIdExpanded(IIncrementalFundsService incrementalFundsService, IMapper mapper)
    {
        _incrementalFundsService = incrementalFundsService;
        _mapper = mapper;
    }

    [HttpGet("api/incrementalFunds/{Id}/expand")]
    [Authorize(Roles = AppRoles.ViewPolicy)]
    [SwaggerOperation(
        Summary = "Get an expanded Incremental Fund by Id",
        Description = "Get an expanded Incremental Fund by Id",
        OperationId = "IncrementalFunds.GetByIdExpanded",
        Tags = new[] { "IncrementalFunds" })]
    public override async Task<ActionResult<IncrementalFundExpandedDto>> HandleAsync(
        [FromRoute] ObjectIdRequest request,
        CancellationToken cancellationToken = new())
    {
        var incrementalFundExpanded = await _incrementalFundsService.GetExpanded(request.Id);
        if (incrementalFundExpanded is null) return NotFound();

        var result = _mapper.Map<IncrementalFundExpandedDto>(incrementalFundExpanded);

        return Ok(result);
    }
}
