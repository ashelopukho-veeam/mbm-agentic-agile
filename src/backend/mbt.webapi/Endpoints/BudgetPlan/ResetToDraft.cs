using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Endpoints.BudgetStructure;
using mbt.webapi.Services;
using mbt.webapi.UseCases.BudgetPlans.Commands;
using mbt.webapi.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.BudgetPlan;

public class ResetToDraft : EndpointBaseAsync.WithRequest<BulkIdsRequest>.WithResult<BulkOperationResult>
{
    private readonly IValidator<BulkIdsRequest> _validator;
    private readonly IMediator _mediator;

    public ResetToDraft(IValidator<BulkIdsRequest> validator, IMediator mediator)
    {
        _validator = validator;
        _mediator = mediator;
    }

    [Authorize(Roles = $"{AppRoles.SysAdmins},{AppRoles.GlobalApprovers}")]
    [HttpPost("api/budgetPlan/resetToDraft")]
    [SwaggerOperation(
        Summary = "Reset a budget plan status to draft",
        Description = "Reset a budget plan status to draft",
        OperationId = "BudgetPlans.ResetToDraft",
        Tags = new[] { "BudgetPlans" })]
    public override async Task<BulkOperationResult> HandleAsync([FromBody] BulkIdsRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var result = await BulkOperation.Run(request.Ids,
            async budgetPlanId =>
            {
                await _mediator.Send(new ResetBudgetPlanCommand { BudgetPlanId = budgetPlanId }, cancellationToken);
            },
            "Reset budget plans status");

        return result;
    }
}
