using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.GroupedActivities;

public class GetById : EndpointBaseAsync.WithRequest<GetByIdGroupedActivityRequest>.WithActionResult<
    GroupedActivityExpanded>
{
    private readonly IGroupedActivityRepository _groupActivitiesRepository;
    private readonly IMapper _mapper;

    public GetById(IMapper mapper, IGroupedActivityRepository groupActivitiesRepository)
    {
        _mapper = mapper;
        _groupActivitiesRepository = groupActivitiesRepository;
    }

    [HttpGet("api/groupedActivities/{GroupedActivityId}")]
    [SwaggerOperation(
        Summary = "Get a Grouped activity by Id",
        Description = "Get a Grouped activity by Id",
        OperationId = "GroupedActivities.GetById",
        Tags = new[] { "GroupedActivities" })]
    public override async Task<ActionResult<GroupedActivityExpanded>> HandleAsync(
        [FromRoute] GetByIdGroupedActivityRequest request,
        CancellationToken cancellationToken = new())
    {
        var groupedActivity = await _groupActivitiesRepository.GetByIdExpanded(request.GroupedActivityId);
        if (groupedActivity is null) return NotFound();

        var result = _mapper.Map<GroupedActivityExpandedDto>(groupedActivity);

        return Ok(result);
    }
}
