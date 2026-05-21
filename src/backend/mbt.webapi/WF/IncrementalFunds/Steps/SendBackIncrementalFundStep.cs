using JetBrains.Annotations;
using mbt.webapi.Services.Interfaces;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace mbt.webapi.WF.IncrementalFunds.Steps;

[PublicAPI]
public class SendBackIncrementalFundStep : StepBody
{
    private readonly IChatService _chatService;
    private readonly IIncrementalFundsService _incrementalFundsService;

    public SendBackIncrementalFundStep(ITaskService taskService, IUserService userService,
        ITransfersService transfersService,
        IChatService chatService,
        IIncrementalFundsService incrementalFundsService
    )
    {
        _incrementalFundsService = incrementalFundsService;
        _chatService = chatService;
    }

    public string IncrementalFundId { get; set; }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        // var t = _incrementalFundsService.Get(IncrementalFundId);
        // t.Status = IncrementalFundStatus.Canceled;
        // _incrementalFundsService.Update(IncrementalFundId, t);

        return ExecutionResult.Next();
    }
}
