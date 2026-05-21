using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.UseCases.BudgetPlans;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.BudgetPlan;

public class GetBudgetPlanById : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithActionResult<BudgetPlanResponse>
{
    private readonly IValidator<ObjectIdRequest> _validator;
    private readonly IMediator _mediator;

    public GetBudgetPlanById(IValidator<ObjectIdRequest> validator, IMediator mediator)
    {
        _validator = validator;
        _mediator = mediator;
    }

    [HttpGet("api/budgetPlan/{Id}")]
    [Authorize(Roles = AppRoles.ViewPolicy)]
    [SwaggerOperation(
        Summary = "Get budget plan by id",
        Description = "Get budget plan by id",
        OperationId = "BudgetPlan.Get",
        Tags = new[] { "BudgetPlans" })]
    public override async Task<ActionResult<BudgetPlanResponse>> HandleAsync([FromRoute] ObjectIdRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        var result = await _mediator.Send(new GetBudgetPlanByIdRequest(request.Id), cancellationToken);


        return result.IsNotFound ? NotFound(ErrorMessages.BudgetPlanNotFound(request.Id)) : Ok(result.Response);
    }
}
