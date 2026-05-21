using System.Threading.Tasks;
using mbt.webapi.Services.Interfaces;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace mbt.webapi.WF.Steps;

public class CancelTransferSubmitStep : StepBodyAsync
{
    private readonly ITaskService _tasksService;

    public CancelTransferSubmitStep(ITaskService tasksService)
    {
        _tasksService = tasksService;
    }

    public string TransferId { get; set; }

    public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        // cancel tasks associated with transfer
        var transferTasks = await _tasksService.GetByAssociatedItemId(TransferId);

        foreach (var task in transferTasks) _tasksService.CancelTask(task.Id);

        return ExecutionResult.Next();
    }
}
