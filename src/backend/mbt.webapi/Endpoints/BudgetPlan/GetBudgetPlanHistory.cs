using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.BuiltIn;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using BudgetPlanHistoryItemExpanded = mbt.webapi.Domain.Entities.BudgetPlanHistoryItemExpanded;

namespace mbt.webapi.Endpoints.BudgetPlan;

public class
    GetBudgetPlanHistory : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithActionResult<
        List<BudgetPlanHistoryItemExpanded>>
{
    private readonly IBudgetService _budgetService;

    public GetBudgetPlanHistory(IBudgetService budgetService)
    {
        _budgetService = budgetService;
    }

    [HttpGet("api/budgetPlan/{Id}/history")]
    [Authorize(Roles = AppRoles.ViewPolicy)]
    [SwaggerOperation(
        Summary = "Get budget plan history",
        Description = "Get budget plans history",
        OperationId = "BudgetPlans.History",
        Tags = new[] { "BudgetPlans" })]
    public override async Task<ActionResult<List<BudgetPlanHistoryItemExpanded>>> HandleAsync(
        [FromRoute] ObjectIdRequest request, CancellationToken cancellationToken = new())
    {
        var budgetPlanHistory = await _budgetService.GetBudgetPlanHistory(request.Id);


        return budgetPlanHistory;
    }
}
