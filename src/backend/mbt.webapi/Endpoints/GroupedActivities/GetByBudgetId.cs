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

public class
    GetByBudgetId : EndpointBaseAsync.WithRequest<GetByBudgetIdRequest>.WithActionResult<List<GroupedActivityDto>>
{
    private readonly IGroupActivitiesService _groupActivitiesService;
    private readonly IMapper _mapper;

    public GetByBudgetId(IGroupActivitiesService groupActivitiesService, IMapper mapper)
    {
        _groupActivitiesService = groupActivitiesService;
        _mapper = mapper;
    }

    [HttpGet("api/groupedActivities/budget/{BudgetId}")]
    [SwaggerOperation(
        Summary = "Get a Grouped activities by Budget Id",
        Description = "Get a Grouped activities by Budget Id",
        OperationId = "GroupedActivities.GetByBudgetId",
        Tags = new[] { "GroupedActivities" })]
    public override async Task<ActionResult<List<GroupedActivityDto>>> HandleAsync(
        [FromRoute] GetByBudgetIdRequest request, CancellationToken cancellationToken = new())
    {
        var groupedActivities = await _groupActivitiesService.GetByBudgetIdAsync(request.BudgetId);
        var result = groupedActivities.Select(_mapper.Map<GroupedActivityDto>);
        return Ok(result);
    }
}
