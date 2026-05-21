using System.Linq;
using JetBrains.Annotations;
using mbt.webapi.Services.Interfaces;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using MbtTaskStatus = mbt.webapi.Domain.Entities.MbtTaskStatus;

namespace mbt.webapi.WF.Steps;

public class CancelActiveTasksStep : StepBody
{
    private readonly ITaskService _taskService;

    public CancelActiveTasksStep(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [UsedImplicitly] public string AssociatedItemId { get; set; }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        var tasks = _taskService.Get().Where(t => t.AssociatedItemId == AssociatedItemId).ToList();
        for (var i = 0; i < tasks.Count(); i++)
        {
            var task = tasks[i];
            if (task.Status == MbtTaskStatus.Pending) _taskService.CancelTask(task.Id);
        }

        return ExecutionResult.Next();
    }
}
