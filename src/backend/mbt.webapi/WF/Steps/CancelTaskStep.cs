using mbt.webapi.Services.Interfaces;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace mbt.webapi.WF.Steps;

public class CancelTaskStep : StepBody
{
    private readonly ITaskService _taskService;

    public CancelTaskStep(ITaskService taskService)
    {
        _taskService = taskService;
    }

    public string TaskId { get; set; }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        var task = _taskService.Get(TaskId);
        if (task != null) _taskService.CancelTask(task.Id);

        return ExecutionResult.Next();
    }
}
