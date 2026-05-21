using System;
using System.Threading;
using System.Threading.Tasks;
using mbt.webapi.Endpoints.BudgetStructure.dto;
using mbt.webapi.Repositories;
using mbt.webapi.Services;
using mbt.webapi.Utils;
using MediatR;

namespace mbt.webapi.UseCases.BudgetStructure.Commands;

public class BulkEditBudgetStructureCommand : IRequest<BulkOperationResult>
{
    public BulkEditRequest Request { get; }

    public BulkEditBudgetStructureCommand(BulkEditRequest request)
    {
        Request = request;
    }
}

public class BulkEditCommandHandler : IRequestHandler<BulkEditBudgetStructureCommand, BulkOperationResult>
{
    private readonly IBudgetRepository _budgetRepository;

    public BulkEditCommandHandler(IBudgetRepository budgetRepository)
    {
        _budgetRepository = budgetRepository;
    }

    public async Task<BulkOperationResult> Handle(BulkEditBudgetStructureCommand budgetStructureCommand, CancellationToken cancellationToken)
    {
        var request = budgetStructureCommand.Request;
        return await BulkOperation.Run(request.Ids,
            async budgetId => await SetUsers(budgetId, request.ParentManagerId, request.ManagerId, request.OwnerId),
            "Bulk edit budget structures");
    }

    private async Task SetUsers(string budgetId, Guid? parentManager, Guid? manager, Guid? owner)
    {
        var budget =
            await _budgetRepository.GetAsync(budgetId) ?? throw new Exception($"Budget not found. {budgetId}");

        if (parentManager != null)
            budget.ParentManagerId = parentManager.ToString();
        if (manager != null) budget.ManagerId = manager.ToString();
        if (owner != null) budget.OwnerId = owner.ToString();

        await _budgetRepository.UpdateAsync(budget);
    }
}
