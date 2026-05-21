using JetBrains.Annotations;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace mbt.webapi.WF.Steps;

[UsedImplicitly]
public class CheckGeoStep : StepBody
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly ITransfersService _transfersService;

    public CheckGeoStep(ITransfersService transfersService, IBudgetRepository budgetRepository)
    {
        _transfersService = transfersService;
        _budgetRepository = budgetRepository;
    }

    public string TransferId { get; set; }

    public bool IsSameGeo { get; set; }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        var transfer = _transfersService.Get(TransferId);
        var fromBudget = _budgetRepository.Get(transfer.FromBudgetId);
        var toBudget = _budgetRepository.Get(transfer.ToBudgetId);


        IsSameGeo = fromBudget.Level1 == toBudget.Level1;

        return ExecutionResult.Next();
    }
}
