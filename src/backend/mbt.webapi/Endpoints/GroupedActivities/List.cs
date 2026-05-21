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

public class ListGroupedActivities : EndpointBaseAsync.WithoutRequest.WithActionResult<List<GroupedActivityDto>>
{
    private readonly IGroupActivitiesService _groupActivitiesService;
    private readonly IMapper _mapper;

    public ListGroupedActivities(IGroupActivitiesService groupActivitiesService, IMapper mapper)
    {
        _groupActivitiesService = groupActivitiesService;
        _mapper = mapper;
    }

    [HttpGet("api/groupedActivities")]
    [SwaggerOperation(
        Summary = "List grouped activities",
        Description = "List grouped activities",
        OperationId = "GroupedActivities.List",
        Tags = new[] { "GroupedActivities" })]
    public override async Task<ActionResult<List<GroupedActivityDto>>> HandleAsync(
        CancellationToken cancellationToken = new())
    {
        var items = await _groupActivitiesService.GetAsync();
        var result = items.Select(_mapper.Map<GroupedActivityDto>);

        return Ok(result);
    }
}
