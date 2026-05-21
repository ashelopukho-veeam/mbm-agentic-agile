using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.BuiltIn;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.GroupedActivities;

public class Delete : EndpointBaseAsync.WithRequest<DeleteGroupedActivityRequest>.WithoutResult
{
    private readonly IGroupActivitiesService _groupActivitiesService;

    public Delete(IGroupActivitiesService groupActivitiesService)
    {
        _groupActivitiesService = groupActivitiesService;
    }


    [HttpDelete("api/groupedActivities/{GroupedActivityId}")]
    [Authorize(Roles = AppRoles.ManageGroupedActivities)]
    [SwaggerOperation(
        Summary = "Deletes a grouped activity",
        Description = "Deletes a grouped activity",
        OperationId = "GroupedActivities.Delete",
        Tags = new[] { "GroupedActivities" })]
    public override async Task<ActionResult> HandleAsync([FromRoute] DeleteGroupedActivityRequest request,
        CancellationToken cancellationToken = new())
    {
        var itemToDelete = await _groupActivitiesService.GetAsync(request.GroupedActivityId);
        if (itemToDelete is null) return NotFound();

        await _groupActivitiesService.RemoveAsync(itemToDelete);

        return Ok();
    }
}
