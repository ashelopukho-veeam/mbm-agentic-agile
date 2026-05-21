using System;
using System.Threading;
using System.Threading.Tasks;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Endpoints.BudgetStructure.dto;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using MediatR;

namespace mbt.webapi.UseCases.BudgetStructure.Commands;

public record CopyBudgetStructureCommand(string BudgetId, int ToYear, string Status) : IRequest<CommandResult<Budget>>;

public class CopyBudgetStructureCommandHandler : IRequestHandler<CopyBudgetStructureCommand, CommandResult<Budget>>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IBudgetService _budgetService;

    public CopyBudgetStructureCommandHandler(IBudgetRepository budgetRepository, IBudgetService budgetService)
    {
        _budgetRepository = budgetRepository;
        _budgetService = budgetService;
    }

    public async Task<CommandResult<Budget>> Handle(CopyBudgetStructureCommand command,
        CancellationToken cancellationToken)
    {
        ValidateCommand(command);

        var budget = await _budgetRepository.GetAsync(command.BudgetId);
        if (budget == null)
            return CommandResult<Budget>.NotFound(ErrorMessages.BudgetNotFound(command.BudgetId));

        ValidateBudgetStructure(budget);

        var request = new CreateBudgetStructureRequest()
        {
            BudgetType = budget.BudgetType,
            Level1 = budget.Level1,
            Level2 = budget.Level2,
            Level3 = budget.Level3,
            CostCenter = budget.CostCenter,
            Year = command.ToYear,
            OwnerId = new Guid(budget.OwnerId),
            ParentManagerId = new Guid(budget.ParentManagerId),
            ManagerId = new Guid(budget.ManagerId),
            IsPaidMedia = budget.IsPaidMedia,
            Status = command.Status
        };

        var newBudget = await _budgetService.CreateAsync(request);

        return CommandResult<Budget>.Success(newBudget);
    }

    private void ValidateCommand(CopyBudgetStructureCommand command)
    {
        if (command.Status != BudgetStatus.InProgress && command.Status != BudgetStatus.ApprovedPlaceholder)
            throw new ApiException(ErrorMessages.CopyBudgetInvalidStatus);
    }

    private void ValidateBudgetStructure(Budget budget)
    {
        if (budget.Status != BudgetStatus.Approved)
            throw new ApiException(ErrorMessages.BudgetIsNotApproved(budget.Title));
    }
}
