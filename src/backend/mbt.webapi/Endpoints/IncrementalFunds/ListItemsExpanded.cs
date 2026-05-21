using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Endpoints.IncrementalFunds.dto;
using mbt.webapi.Services;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.IncrementalFunds;

public class
    ListIncrementalFundsExpanded : EndpointBaseAsync.WithRequest<ListIncrementalFundsRequest>.WithActionResult<
    List<IncrementalFundExpanded>>
{
    private readonly IIncrementalFundsService _incrementalFundsService;
    private readonly IValidator<ListIncrementalFundsRequest> _validator;

    public ListIncrementalFundsExpanded(IIncrementalFundsService incrementalFundsService,
        IValidator<ListIncrementalFundsRequest> validator)
    {
        _incrementalFundsService = incrementalFundsService;
        _validator = validator;
    }

    [HttpGet("api/incrementalFunds/expand")]
    [Authorize(Roles = AppRoles.ViewPolicy)]
    [SwaggerOperation(
        Summary = "List expanded incremental funds",
        Description = "List expanded incremental funds",
        OperationId = "IncrementalFunds.ListExpanded",
        Tags = new[] { "IncrementalFunds" })]
    public override async Task<ActionResult<List<IncrementalFundExpanded>>> HandleAsync(
        [FromQuery] ListIncrementalFundsRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);

        var items = await _incrementalFundsService.GetExpanded(request.Year, request.Plan);

        return items;
    }
}
