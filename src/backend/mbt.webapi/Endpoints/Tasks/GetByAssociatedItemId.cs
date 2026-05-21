using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.Tasks;

public class
    GetAssociatedItemId : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithResult<List<MbtTaskExpanded>>
{
    private readonly ITasksRepository _taskRepository;

    public GetAssociatedItemId(ITasksRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    [HttpGet("api/tasks/associatedItemId/{Id}")]
    [Authorize(Roles = AppRoles.ViewPolicy)]
    [SwaggerOperation(
        Summary = "Get tasks by associated item Id",
        Description = "Get tasks by associated item Id",
        OperationId = "Tasks.GetByAssociatedItemId",
        Tags = new[] { "Tasks" })]
    public override async Task<List<MbtTaskExpanded>> HandleAsync([FromRoute] ObjectIdRequest request,
        CancellationToken cancellationToken = new())
    {
        var filter = new ExpressionFilterDefinition<MbtTaskExpanded>(t => t.AssociatedItemId == request.Id);
        var associatedTasks = await _taskRepository.GetExpanded(filter);


        return associatedTasks;
    }
}
