using System.Collections.Generic;
using System.Threading.Tasks;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Endpoints.BudgetStructure.dto;

namespace mbt.webapi.Services.Interfaces;

public interface IBudgetService : IBaseService
{
    Task<Budget> GetBudgetByPlanId(string planId);
    Task<List<Budget>> GetByYear(int year);
    Task<List<BudgetPlanHistoryItemExpanded>> GetBudgetPlanHistory(string planId);
    Task<Budget> UpdateAsync(Budget budget);
    Task<Budget> CreateAsync(CreateBudgetStructureRequest budget);
}
