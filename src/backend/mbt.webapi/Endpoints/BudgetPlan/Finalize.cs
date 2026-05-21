using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.BuiltIn;
using mbt.webapi.UseCases.BudgetStructure.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.BudgetPlan;

public class Finalize : EndpointBaseAsync.WithoutRequest.WithoutResult
{
    private readonly IMediator _mediator;

    public Finalize(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = $"{AppRoles.SysAdmins},{AppRoles.GlobalApprovers}")]
    [HttpPost(BudgetPlanRoutes.Finalize)]
    [SwaggerOperation(
        Summary = "Finalize budget plans",
        Description = "Finalize budget plans",
        OperationId = "BudgetPlans.Finalize",
        Tags = new[] { "BudgetPlans" })]
    public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = new())
    {
        await _mediator.Send(new FinalizeBudgetPlansCommand(), cancellationToken);

        return Ok();
    }
}
