using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.GroupedActivities;

public class GetByBudgetPlanIdExpanded : EndpointBaseAsync.WithRequest<GetByBudgetPlanIdRequest>.WithActionResult<
    List<GroupedActivityExpanded>>
{
    private readonly IGroupActivitiesService _groupActivitiesService;

    public GetByBudgetPlanIdExpanded(IGroupActivitiesService groupActivitiesService)
    {
        _groupActivitiesService = groupActivitiesService;
    }

    [HttpGet("api/groupedActivities/budgetPlan/{BudgetPlanId}/expand")]
    [SwaggerOperation(
        Summary = "Get a Grouped activities (expanded) by Budget Plan Id",
        Description = "Get a Grouped activities (expanded) by Budget Plan Id",
        OperationId = "GroupedActivities.GetByBudgetPlanIdExpanded",
        Tags = new[] { "GroupedActivities" })]
    public override async Task<ActionResult<List<GroupedActivityExpanded>>> HandleAsync(
        [FromRoute] GetByBudgetPlanIdRequest request, CancellationToken cancellationToken = new())
    {
        var groupedActivities = await _groupActivitiesService.GetByBudgetPlanIdExpanded(request.BudgetPlanId);
        return groupedActivities;
    }
}
