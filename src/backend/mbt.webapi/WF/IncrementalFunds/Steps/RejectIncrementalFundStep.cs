using mbt.webapi.Services;
using mbt.webapi.Services.Interfaces;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace mbt.webapi.WF.IncrementalFunds.Steps;

public class RejectIncrementalFundStep : StepBody
{
    private readonly IChatService _chatService;
    private readonly IIncrementalFundsService _incrementalFundsService;
    private readonly IMailService _mailService;

    public RejectIncrementalFundStep(ITaskService taskService, IUserService userService,
        ITransfersService transfersService,
        IChatService chatService,
        IIncrementalFundsService incrementalFundsService, IMailService mailService)
    {
        _incrementalFundsService = incrementalFundsService;
        _mailService = mailService;
        _chatService = chatService;
    }

    public string IncrementalFundId { get; set; }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        return ExecutionResult.Next();
    }
}
