using JetBrains.Annotations;
using mbt.webapi.Services.Interfaces;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using TransferStatus = mbt.webapi.Domain.Entities.TransferStatus;

namespace mbt.webapi.WF.Steps;

[PublicAPI]
public class ApproveTransferStep : StepBody
{
    private readonly IChatService _chatService;
    private readonly ITransfersService _transfersService;

    public ApproveTransferStep(ITaskService taskService, IUserService userService,
        ITransfersService transfersService,
        IChatService chatService
    )
    {
        _transfersService = transfersService;
        _chatService = chatService;
    }

    public string TransferId { get; set; }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        var t = _transfersService.Get(TransferId);
        t.Status = TransferStatus.Approved;
        _transfersService.Update(t);
        _chatService.AddSystemChatMessage(TransferId, "Transfer is approved");

        return ExecutionResult.Next();
    }
}
