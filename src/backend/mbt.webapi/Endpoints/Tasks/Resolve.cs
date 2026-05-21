using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WorkflowCore.Interface;
using MbtTaskResolveAction = mbt.webapi.Domain.Entities.MbtTaskResolveAction;
using MbtTaskStatus = mbt.webapi.Domain.Entities.MbtTaskStatus;

namespace mbt.webapi.Endpoints.Tasks;

public class Resolve : EndpointBaseAsync.WithRequest<ResolveTaskRequest>.WithActionResult<MbtTaskExpanded>
{
    private readonly ITaskService _taskService;
    private readonly IWorkflowController _workflowService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly ITasksRepository _taskRepository;
    private readonly IValidator<ResolveTaskRequest> _validator;


    public Resolve(ITaskService taskService, IWorkflowController workflowService,
        ICurrentUserContext currentUserContext, ITasksRepository taskRepository,
        IValidator<ResolveTaskRequest> validator)
    {
        _taskService = taskService;
        _workflowService = workflowService;
        _currentUserContext = currentUserContext;
        _taskRepository = taskRepository;
        _validator = validator;
    }

    [HttpPost("api/tasks/resolve")]
    [Authorize(Roles = AppRoles.ViewPolicy)]
    [SwaggerOperation(
        Summary = "Resolve a Task by Id",
        Description = "Resolve a Task by Id",
        OperationId = "Tasks.Resolve",
        Tags = new[] { "Tasks" })]
    public override async Task<ActionResult<MbtTaskExpanded>> HandleAsync([FromBody] ResolveTaskRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var outcome = request.Status switch
        {
            MbtTaskResolveAction.Approve => MbtTaskStatus.Approved,
            MbtTaskResolveAction.Reject => MbtTaskStatus.Rejected,
            MbtTaskResolveAction.SendBackToEdit => MbtTaskStatus.SendBackToEdit,
            _ => throw new ApiException("Task action isn't supported: " + request.Status)
        };

        var task = await _taskService.GetAsync(request.Id);

        if (task == null) return NotFound();

        // check if admin or assignedTo
        var isAdmin = _currentUserContext.IsInRoles(new[] { AppRoles.Admins, AppRoles.SysAdmins });
        var isAssignedTo = _currentUserContext.UserId == task.AssignedTo;
        if (!isAdmin && !isAssignedTo)
            throw new AccessDeniedException();

        await _taskService.Resolve(task.Id, request.Comment, outcome);

        //ResolveTask
        await _workflowService.PublishEvent(WorkflowEventNames.ResolveTask, task.Id, outcome);

        var expandedTask = await _taskRepository.GetByIdExpanded(task.Id);
        return expandedTask;
    }
}
