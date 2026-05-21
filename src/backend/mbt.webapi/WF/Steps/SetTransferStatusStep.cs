using JetBrains.Annotations;
using mbt.webapi.Services.Interfaces;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace mbt.webapi.WF.Steps;

[PublicAPI]
public class SetTransferStatusStep : StepBody
{
    private readonly ITransfersService _transferService;

    public SetTransferStatusStep(ITransfersService transfersService)
    {
        _transferService = transfersService;
    }

    public string TransferId { get; set; }
    public string Status { get; set; }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        var transfer = _transferService.Get(TransferId);
        transfer.Status = Status;
        _transferService.Update(transfer);
        return ExecutionResult.Next();
    }
}
