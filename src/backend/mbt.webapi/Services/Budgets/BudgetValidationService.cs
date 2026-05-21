using System;
using System.Linq;
using System.Threading.Tasks;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using mbt.webapi.Shared;
using MongoDB.Driver;

namespace mbt.webapi.Services.Budgets;

public interface IBudgetValidationService : IBaseService
{
    Task ValidateBudgetTitle(Budget budget);
    Task ValidateUnprocessedTransfersAndIncrementalFunds(Budget budget);
    Task ValidateUnprocessedTransfersAndIncrementalFunds(Period period);
}

public class BudgetValidationService : IBudgetValidationService
{
    private readonly IBudgetRepository _budgetsRepository;
    private readonly ITransfersRepository _transfersRepository;
    private readonly IIncrementalFundsRepository _incrementalFundsRepository;

    private readonly string[] _validTransferStatuses =
    {
        TransferStatus.Approved,
        TransferStatus.Rejected,
        TransferStatus.Canceled
    };

    private readonly string[] _validIncrementalFundStatuses =
    {
        IncrementalFundStatus.Approved,
        IncrementalFundStatus.Rejected,
        IncrementalFundStatus.Canceled
    };

    public BudgetValidationService(IBudgetRepository budgetsRepository,
        IIncrementalFundsRepository incrementalFundsRepository, ITransfersRepository transfersRepository)
    {
        _budgetsRepository = budgetsRepository;
        _incrementalFundsRepository = incrementalFundsRepository;
        _transfersRepository = transfersRepository;
    }

    public async Task ValidateBudgetTitle(Budget budget)
    {
        var budgetWithSameTitle =
            await _budgetsRepository.FindOneAsync(b => b.Title == budget.Title && b.Id != budget.Id);
        if (budgetWithSameTitle != null)
            throw new DuplicateItemException(ErrorMessages.DuplicateBudgetTitle(budget.Title));
    }

    public async Task ValidateUnprocessedTransfersAndIncrementalFunds(Period period)
    {
        var invalidTransfersFilter = BuildTransferValidationFilter(period);
        var invalidTransfersCount = await _transfersRepository.CountAsync(invalidTransfersFilter);

        var invalidIncrementalFundsFilter = BuildIncrementalFundValidationFilter(period);
        var invalidIncrementalFundsCount = await _incrementalFundsRepository.CountAsync(invalidIncrementalFundsFilter);

        if (invalidTransfersCount > 0 || invalidIncrementalFundsCount > 0)
            throw new ApiException(ErrorMessages.PeriodHasUnprocessedTransfersOrIncFunds);
    }

    public async Task ValidateUnprocessedTransfersAndIncrementalFunds(Budget budget)
    {
        var transfersFilter = BuildTransferValidationFilter(budget);
        var transfersCount = await _transfersRepository.CountAsync(transfersFilter);

        var incrementalFundsFilter = BuildIncrementalFundValidationFilter(budget);
        var incrementalFundsCount = await _incrementalFundsRepository.CountAsync(incrementalFundsFilter);

        if (transfersCount > 0 || incrementalFundsCount > 0)
            throw new ApiException(
                ErrorMessages.BudgetHasUnprocessedTransfersOrIncFunds(budget.Title));
    }

    private FilterDefinition<Transfer> BuildTransferValidationFilter(Period period) =>
        new FilterDefinitionBuilder<Transfer>()
            .Where(t => t.Year == period.Year &&
                        t.Plan == period.PlanName &&
                        !_validTransferStatuses.Contains(t.Status));

    private FilterDefinition<IncrementalFund> BuildIncrementalFundValidationFilter(Period period) =>
        new FilterDefinitionBuilder<IncrementalFund>()
            .Where(i => i.Year == period.Year &&
                        i.Plan == period.PlanName &&
                        !_validIncrementalFundStatuses.Contains(i.Status));


    private FilterDefinition<IncrementalFund> BuildIncrementalFundValidationFilter(Budget budget) =>
        new FilterDefinitionBuilder<IncrementalFund>()
            .Where(i => i.ToBudgetId == budget.Id &&
                        !_validIncrementalFundStatuses.Contains(i.Status));

    private FilterDefinition<Transfer> BuildTransferValidationFilter(Budget budget) =>
        new FilterDefinitionBuilder<Transfer>()
            .Where(t => (t.FromBudgetId == budget.Id || t.ToBudgetId == budget.Id) &&
                        !_validTransferStatuses.Contains(t.Status));



}
