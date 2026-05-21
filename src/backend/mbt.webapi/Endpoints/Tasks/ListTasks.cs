using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using mbt.webapi.Services;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.Tasks;

public class ListTasks : EndpointBaseAsync.WithRequest<string>.WithResult<List<MbtTaskExpanded>>
{
    private readonly ITasksRepository _taskRepository;
    private readonly ICurrentUserContext _currentUserContext;


    public ListTasks(ICurrentUserContext currentUserContext, ITasksRepository taskRepository)
    {
        _currentUserContext = currentUserContext;
        _taskRepository = taskRepository;
    }

    [HttpGet("api/tasks")]
    [Authorize(Roles = AppRoles.ViewPolicy)]
    [SwaggerOperation(
        Summary = "List tasks",
        Description = "List tasks",
        OperationId = "Tasks.List",
        Tags = new[] { "Tasks" })]
    public override async Task<List<MbtTaskExpanded>> HandleAsync([FromQuery] [CanBeNull] string status,
        CancellationToken cancellationToken = new())
    {
        var isAdmin = _currentUserContext.IsInRoles(new[] { AppRoles.Admins, AppRoles.SysAdmins });

        var filter =
            string.IsNullOrWhiteSpace(status)
                ? FilterDefinition<MbtTaskExpanded>.Empty
                : new ExpressionFilterDefinition<MbtTaskExpanded>(t => t.Status == status);

        if (!isAdmin)
        {
            filter &= new ExpressionFilterDefinition<MbtTaskExpanded>(t => t.AssignedTo == _currentUserContext.UserId);
        }

        var tasks = await _taskRepository.GetExpanded(filter);

        return tasks;
    }
}
