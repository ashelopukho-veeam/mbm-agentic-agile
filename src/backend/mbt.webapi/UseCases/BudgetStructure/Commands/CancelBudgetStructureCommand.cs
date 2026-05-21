using System.Threading;
using System.Threading.Tasks;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Budgets;
using MediatR;

namespace mbt.webapi.UseCases.BudgetStructure.Commands;

public class CancelBudgetStructureCommand : IRequest<Budget>
{
    public CancelBudgetStructureCommand(string id)
    {
        Id = id;
    }

    public string Id { get; }
}

public class CancelBudgetStructureHandler : IRequestHandler<CancelBudgetStructureCommand, Budget>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IBudgetValidationService _budgetValidationService;

    public CancelBudgetStructureHandler(IBudgetRepository budgetRepository,
        IBudgetValidationService budgetValidationService)
    {
        _budgetRepository = budgetRepository;
        _budgetValidationService = budgetValidationService;
    }

    public async Task<Budget> Handle(CancelBudgetStructureCommand command, CancellationToken cancellationToken)
    {
        var budgetId = command.Id;

        var budget = await _budgetRepository.GetAsync(budgetId);
        if (budget == null)
            throw new ApiException(ErrorMessages.BudgetNotFound(budgetId));

        if (budget.Status != BudgetStatus.Approved &&
            budget.Status != BudgetStatus.ApprovedPlaceholder)
            throw new ApiException(ErrorMessages.BudgetIsNotApprovedOrApprovedPlaceHolder(budget.Title));

        await _budgetValidationService.ValidateUnprocessedTransfersAndIncrementalFunds(budget);

        budget.Status = BudgetStatus.Canceled;
        budget.UseInCoupa = false;
        budget.UseInTableau = false;
        await _budgetRepository.UpdateAsync(budget);

        return budget;
    }
}
