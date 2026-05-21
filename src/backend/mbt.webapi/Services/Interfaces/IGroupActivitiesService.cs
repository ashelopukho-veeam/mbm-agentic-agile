using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Endpoints.GroupedActivities;
using Microsoft.AspNetCore.Http;

namespace mbt.webapi.Services.Interfaces;

public interface IGroupActivitiesService : IBaseService
{
    Task<List<GroupedActivity>> GetByBudgetPlanId(string id);
    Task<List<GroupedActivityExpanded>> GetByBudgetPlanIdExpanded(string id);
    Task<List<GroupedActivity>> GetByBudgetIdAsync(string id);
    Task ImportFromCsv(Budget budget, string budgetPlanId, IFormFile formFile);
    Task<BudgetPlanGroupedActivitySummary> GetSummaryReport(string budgetPlanId);
    Task<List<GroupedActivity>> GetAsync();
    Task<GroupedActivity> GetAsync(string id);
    Task RemoveAsync(GroupedActivity ga);
    Task<GroupedActivity> Clone(GroupedActivity groupedActivity, string toPlanId);
    Task Clone(string fromPlanId, string toPlanId);
    void RemoveOldItems(TimeSpan fromDays);
}
