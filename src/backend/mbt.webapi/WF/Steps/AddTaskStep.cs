using mbt.webapi.Services.Interfaces;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using MbtTask = mbt.webapi.Domain.Entities.MbtTask;
using MbtTaskStatus = mbt.webapi.Domain.Entities.MbtTaskStatus;

namespace mbt.webapi.WF.Steps;

public class AddTaskStep : StepBody
{
    private readonly ITaskService _taskService;
    private readonly IUserService _userService;

    public AddTaskStep(ITaskService taskService, IUserService userService)
    {
        _taskService = taskService;
        _userService = userService;
    }

    public string Message { get; set; }
    public string Details { get; set; }

    public string AssignedTo { get; set; }
    public string AssociatedItemId { get; set; }

    public string Outcome { get; set; }

    public string TaskType { get; set; }

    public string TaskId { get; set; }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        var assignedUser = _userService.Get(AssignedTo);

        var task = new MbtTask();
        task.AssignedTo = assignedUser.Id;
        task.Title = Message;
        task.AssociatedItemId = AssociatedItemId;
        task.Details = Details;
        task.Status = MbtTaskStatus.Pending;
        task.Type = TaskType;

        var t = _taskService.Create(task);
        TaskId = t.Id;


        return ExecutionResult.Next();
    }
}
