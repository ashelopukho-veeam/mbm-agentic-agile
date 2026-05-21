using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.Tasks;

public class GetById : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithActionResult<MbtTaskExpanded>
{
    private readonly ITasksRepository _taskRepository;
    private readonly IValidator<ObjectIdRequest> _validator;

    public GetById(ITasksRepository taskRepository, IValidator<ObjectIdRequest> validator)
    {
        _taskRepository = taskRepository;
        _validator = validator;
    }

    [HttpGet("api/tasks/{Id}")]
    [Authorize(Roles = AppRoles.ViewPolicy)]
    [SwaggerOperation(
        Summary = "Get a Task by Id",
        Description = "Get a Task by Id",
        OperationId = "Tasks.GetById",
        Tags = new[] { "Tasks" })]
    public override async Task<ActionResult<MbtTaskExpanded>> HandleAsync([FromRoute] ObjectIdRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var task = await _taskRepository.GetByIdExpanded(request.Id);
        return task == null ? NotFound() : task;
    }
}
