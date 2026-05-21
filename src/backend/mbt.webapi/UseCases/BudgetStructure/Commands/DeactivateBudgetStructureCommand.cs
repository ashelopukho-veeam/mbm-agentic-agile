using System.Threading;
using System.Threading.Tasks;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Budgets;
using mbt.webapi.Services.Interfaces;
using MediatR;

namespace mbt.webapi.UseCases.BudgetStructure.Commands;

public class DeactivateBudgetStructureCommand : IRequest<Budget>
{
    public DeactivateBudgetStructureCommand(string id)
    {
        Id = id;
    }

    public string Id { get; }
}

public class DeactivateBudgetStructureHandler : IRequestHandler<DeactivateBudgetStructureCommand, Budget>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IApiService _apiService;
    private readonly IBudgetValidationService _budgetValidationService;

    public DeactivateBudgetStructureHandler(IBudgetRepository budgetRepository,
        IApiService apiService, IBudgetValidationService budgetValidationService)
    {
        _budgetRepository = budgetRepository;
        _apiService = apiService;
        _budgetValidationService = budgetValidationService;
    }

    public async Task<Budget> Handle(DeactivateBudgetStructureCommand command, CancellationToken cancellationToken)
    {
        var budgetId = command.Id;

        var budget = await _budgetRepository.GetAsync(budgetId);
        if (budget == null)
            throw new ApiException(ErrorMessages.BudgetNotFound(budgetId));

        if (budget.Status != BudgetStatus.Approved)
            throw new ApiException(ErrorMessages.BudgetIsNotApproved(budget.Title));

        var activeForecastPeriod = await _apiService.GetActiveForecastPeriod();
        if (budget.Year > activeForecastPeriod.Year - 1)
        {
            throw new ApiException(ErrorMessages.BudgetDeactivationIsProhibited(budget.Year));
        }

        await _budgetValidationService.ValidateUnprocessedTransfersAndIncrementalFunds(budget);

        budget.Status = BudgetStatus.Inactive;
        budget.UseInCoupa = false;
        budget.UseInTableau = true;
        await _budgetRepository.UpdateAsync(budget);

        return budget;
    }
}
