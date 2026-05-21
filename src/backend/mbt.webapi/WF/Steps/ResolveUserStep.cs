using mbt.webapi.Services.Interfaces;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using UserProfile = mbt.webapi.Domain.Entities.UserProfile;

namespace mbt.webapi.WF.Steps;

public class ResolveUserStep : StepBody
{
    private readonly IUserService _userService;

    public ResolveUserStep(IUserService userService)
    {
        _userService = userService;
    }

    public string UserId { get; set; }

    public UserProfile User { get; set; }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        User = _userService.Get(UserId);

        return ExecutionResult.Next();
    }
}
