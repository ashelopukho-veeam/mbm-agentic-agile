using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.BuiltIn;
using mbt.webapi.UseCases.BudgetStructure.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.BudgetStructure;

public class GetYears : EndpointBaseAsync.WithoutRequest.WithActionResult<int[]>
{
    private readonly IMediator _mediator;

    public GetYears(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("api/budgetStructure/years")]
    [Authorize(Roles = AppRoles.ViewPolicy)]
    [SwaggerOperation(
        Summary = "Get a list of years",
        Description = "Get a list of years",
        OperationId = "BudgetStructure.GetYears",
        Tags = new[] { "BudgetStructure" })]
    public override async Task<ActionResult<int[]>> HandleAsync(
        CancellationToken cancellationToken = new())
    {
        var years = await _mediator.Send(new BudgetStructureGetYearsQuery(), cancellationToken);

        return Ok(years);
    }
}
