using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Budgets;
using mbt.webapi.Services.Interfaces;
using mbt.webapi.Shared;
using mbt.webapi.Utils;
using MediatR;
using MongoDB.Driver;

namespace mbt.webapi.UseCases.BudgetStructure.Commands;

public class FinalizeBudgetPlansCommand : IRequest
{
}

public class FinalizeRequestHandler : IRequestHandler<FinalizeBudgetPlansCommand>
{
    private readonly IGroupActivitiesService _groupActivitiesService;
    private readonly IApiService _apiService;
    private readonly IBudgetRepository _budgetsRepository;
    private readonly IBudgetValidationService _budgetValidationService;
    private readonly ICurrentUserContext _currentUserContext;

    public FinalizeRequestHandler(
        IApiService apiService,
        IGroupActivitiesService groupActivitiesService, IBudgetRepository budgetsRepository,
        IBudgetValidationService budgetValidationService, ICurrentUserContext currentUserContext)
    {
        _apiService = apiService;
        _groupActivitiesService = groupActivitiesService;
        _budgetsRepository = budgetsRepository;
        _budgetValidationService = budgetValidationService;
        _currentUserContext = currentUserContext;
    }

    public async Task Handle(FinalizeBudgetPlansCommand budgetPlansCommand, CancellationToken cancellationToken)
    {
        var currentActiveForecastPeriod = GetCurrentForecastPeriod();
        await FinalizeBudgetPlans(currentActiveForecastPeriod);
        await MoveToNextPeriod(currentActiveForecastPeriod.Next());
    }

    private Period GetCurrentForecastPeriod()
    {
        var config = _apiService.GetBudgetPlanConfig().Result;
        return new Period(config.CurrentBudgetPlanYear, config.CurrentBudgetPlanName);
    }

    private async Task CloneGroupActivities(Budget budget, Period periodFrom)
    {
        _currentUserContext.UseSystemAccount = true;
        try
        {
            if (periodFrom.Plan >= BuiltInConstants.ForecastsPerYear) return;

            var fromPlanId = budget.Plans[periodFrom.Plan - 1].Id;
            var toPlanId = budget.Plans[periodFrom.Next().Plan - 1].Id;
            await _groupActivitiesService.Clone(fromPlanId, toPlanId);
        }
        finally
        {
            _currentUserContext.UseSystemAccount = false;
        }
    }

    private Task MoveToNextPeriod(Period nextPeriod)
    {
        return _apiService.SetBudgetPlanConfig(new BudgetPlanConfig()
        {
            CurrentBudgetPlanName = nextPeriod.PlanName,
            CurrentBudgetPlanYear = nextPeriod.Year
        });
    }


    private async Task ValidatePlans(Period period)
    {
        var filter = Builders<Budget>.Filter.Eq(b => b.Year, period.Year) &
                     Builders<Budget>.Filter.Eq(b => b.Status, BudgetStatus.Approved) &
                     Builders<Budget>.Filter.Nin(b => b.Plans[period.Plan - 1].Status,
                         new[] { BudgetPlanStatus.Approved, BudgetPlanStatus.Final });

        var count = await _budgetsRepository.CountAsync(filter);

        if (count > 0) throw new InvalidOperationException(ErrorMessages.NotAllPlansApproved);
    }

    private async Task ValidateFinalization(Period period)
    {
        await _budgetValidationService.ValidateUnprocessedTransfersAndIncrementalFunds(period.Previous());
        await ValidatePlans(period);
    }

    private async Task FinalizeBudgetPlans(Period periodToFinalize)
    {
        await ValidateFinalization(periodToFinalize);
        var budgetsToFinalize = await GetBudgetsToFinalize(periodToFinalize);

        foreach (var budget in budgetsToFinalize)
        {
            await ProcessBudgetFinalization(budget, periodToFinalize.PlanName);
            await CloneGroupActivities(budget, periodToFinalize);
        }
    }

    private async Task<List<Budget>> GetBudgetsToFinalize(Period periodToFinalize)
    {
        var planIndex = periodToFinalize.Plan - 1;

        var filter = Builders<Budget>.Filter.Eq(b => b.Year, periodToFinalize.Year) &
                     Builders<Budget>.Filter.Eq(b => b.Status, BudgetStatus.Approved) &
                     Builders<Budget>.Filter.Eq(b => b.Plans[planIndex].Status,
                         BudgetPlanStatus.Approved);

        var budgetsToFinalize = await _budgetsRepository.FindAsync(filter);

        return budgetsToFinalize;
    }

    private async Task ProcessBudgetFinalization(Budget budget, string planName)
    {
        var budgetPlan = budget.Plans.First(p => p.Quarter == planName);
        budgetPlan.Status = BudgetPlanStatus.Final;

        // copy finalized budget plan data to the next period
        var nextPlanName = QuarterUtils.GetNextQuarterName(budgetPlan.Quarter);
        if (!string.IsNullOrEmpty(nextPlanName))
        {
            var nextPlan = budget.GetPlanByQuarter(nextPlanName);
            CopyBudgetPlanData(budgetPlan, nextPlan);
        }

        await _budgetsRepository.UpdateAsync(budget);
    }

    private void CopyBudgetPlanData(BudgetPlan source, BudgetPlan target)
    {
        target.Q1 = source.Q1;
        target.Q2 = source.Q2;
        target.Q3 = source.Q3;
        target.Q4 = source.Q4;
        target.Segments = source.Segments.Select(s => new TitleNumberValuePair { Title = s.Title, Value = s.Value }).ToList();
        target.Campaigns = source.Campaigns.Select(c => new TitleNumberValuePair { Title = c.Title, Value = c.Value }).ToList();
    }
}
