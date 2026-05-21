using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Endpoints.BudgetStructure.dto;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using MongoDB.Driver;

namespace mbt.webapi.Services.Budgets;

[UsedImplicitly]
public class BudgetService : IBudgetService
{
    private readonly IBudgetRepository _budgetsRepository;
    private readonly IBudgetPlanHistoryRepository _budgetPlanHistoryRepository;

    private readonly IBudgetConstructionService _budgetConstructionService;
    private readonly IBudgetValidationService _budgetValidationService;


    public BudgetService(
        IBudgetRepository budgetsRepository,
        IBudgetPlanHistoryRepository budgetPlanHistoryRepository,
        IBudgetConstructionService budgetConstructionService,
        IBudgetValidationService budgetValidationService
    )
    {
        _budgetsRepository = budgetsRepository;
        _budgetConstructionService = budgetConstructionService;
        _budgetValidationService = budgetValidationService;
        _budgetPlanHistoryRepository = budgetPlanHistoryRepository;
    }

    public async Task<Budget> GetBudgetByPlanId(string planId)
    {
        var budget = await _budgetsRepository.FindOneAsync(b => b.Plans.Any(p => p.Id == planId));
        return budget;
    }

    public Task<List<Budget>> GetByYear(int year)
    {
        return _budgetsRepository.FindAsync(b => b.Year == year);
    }

    public async Task<List<BudgetPlanHistoryItemExpanded>> GetBudgetPlanHistory(string planId)
    {
        var filter = new ExpressionFilterDefinition<BudgetPlanHistoryItemExpanded>(b => b.OriginalId == planId);
        var result = await _budgetPlanHistoryRepository.GetExpanded(filter);

        return result;
    }

    public async Task<Budget> UpdateAsync(Budget budget)
    {
        await _budgetValidationService.ValidateBudgetTitle(budget);
        await _budgetsRepository.UpdateAsync(budget);

        return budget;
    }


    public async Task<Budget> CreateAsync(CreateBudgetStructureRequest request)
    {
        var budget = await _budgetConstructionService.BuildBudget(request);
        await _budgetValidationService.ValidateBudgetTitle(budget);

        var result = await _budgetsRepository.CreateAsync(budget);
        return result;
    }
}
