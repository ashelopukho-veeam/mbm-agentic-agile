using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.GroupedActivities;

public class GetByBudgetPlanId : EndpointBaseAsync.WithRequest<GetByBudgetPlanIdRequest>.WithActionResult<
    List<GroupedActivityDto>>
{
    private readonly IGroupActivitiesService _groupActivitiesService;
    private readonly IMapper _mapper;

    public GetByBudgetPlanId(IGroupActivitiesService groupActivitiesService, IMapper mapper)
    {
        _groupActivitiesService = groupActivitiesService;
        _mapper = mapper;
    }

    [HttpGet("api/groupedActivities/budgetPlan/{BudgetPlanId}")]
    [SwaggerOperation(
        Summary = "Get a Grouped activities by Budget Plan Id",
        Description = "Get a Grouped activities by Budget Plan Id",
        OperationId = "GroupedActivities.GetByBudgetPlanId",
        Tags = new[] { "GroupedActivities" })]
    public override async Task<ActionResult<List<GroupedActivityDto>>> HandleAsync(
        [FromRoute] GetByBudgetPlanIdRequest request, CancellationToken cancellationToken = new())
    {
        var groupedActivities = await _groupActivitiesService.GetByBudgetPlanId(request.BudgetPlanId);
        var result = groupedActivities.Select(_mapper.Map<GroupedActivityDto>);
        return Ok(result);
    }
}
