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

public class GetById : EndpointBaseAsync.WithRequest<GetByIdIncrementalFundRequest>.WithActionResult<IncrementalFundDto>
{
    private readonly IIncrementalFundsService _incrementalFundsService;
    private readonly IMapper _mapper;

    public GetById(IIncrementalFundsService incrementalFundsService, IMapper mapper)
    {
        _incrementalFundsService = incrementalFundsService;
        _mapper = mapper;
    }

    [HttpGet("api/incrementalFunds/{IncrementalFundId}")]
    [Authorize(Roles = AppRoles.ViewPolicy)]
    [SwaggerOperation(
        Summary = "Get an Incremental Fund by Id",
        Description = "Get an Incremental Fund by Id",
        OperationId = "IncrementalFunds.GetById",
        Tags = new[] { "IncrementalFunds" })]
    public override async Task<ActionResult<IncrementalFundDto>> HandleAsync(
        [FromRoute] GetByIdIncrementalFundRequest request,
        CancellationToken cancellationToken = new())
    {
        var incrementalFund = await _incrementalFundsService.GetAsync(request.IncrementalFundId);
        if (incrementalFund is null) return NotFound();

        var result = _mapper.Map<IncrementalFundDto>(incrementalFund);

        return Ok(result);
    }
}
