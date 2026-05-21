using System.Collections.Generic;
using System.Linq;
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

public class GetByYear : EndpointBaseAsync.WithRequest<GetByYearIncrementalFundsRequest>.WithActionResult<
    List<IncrementalFundDto>>
{
    private readonly IIncrementalFundsService _incrementalFundsService;
    private readonly IMapper _mapper;

    public GetByYear(IIncrementalFundsService incrementalFundsService, IMapper mapper)
    {
        _incrementalFundsService = incrementalFundsService;
        _mapper = mapper;
    }

    [HttpGet("api/incrementalFunds/getByYear/{Year}")]
    [Authorize(Roles = AppRoles.ViewPolicy)]
    [SwaggerOperation(
        Summary = "Get Incremental Funds by Year",
        Description = "Get Incremental Funds by Year",
        OperationId = "IncrementalFunds.GetByYear",
        Tags = new[] { "IncrementalFunds" })]
    public override async Task<ActionResult<List<IncrementalFundDto>>> HandleAsync(
        [FromRoute] GetByYearIncrementalFundsRequest request,
        CancellationToken cancellationToken = new())
    {
        var incrementalFunds = await _incrementalFundsService.GetByYearAsync(request.Year);

        var result = incrementalFunds.Select(_mapper.Map<IncrementalFundDto>);

        return Ok(result);
    }
}
