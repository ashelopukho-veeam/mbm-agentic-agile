using System.Collections.Generic;
using System.Threading.Tasks;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Shared;

namespace mbt.webapi.Services.Interfaces;

public interface IApiService : IBaseService
{
    Task<List<BudgetLevel>> GetBudgetLevels();
    Task<List<CostCenter>> GetCostCenters();
    Task<List<CostCenter>> SetCostCenters(List<CostCenter> costCenters);
    AppConfig GetAppConfig();
    Task<AppConfig> GetAppConfigAsync();
    Task SetAppConfigAsync(AppConfig config);
    CommonConfig GetCommonConfig();
    Task<CommonConfig> GetCommonConfigAsync();

    Task SetCommonConfigAsync(CommonConfig config
    );

    Task<BudgetPlanConfig> GetBudgetPlanConfig();
    Task SetBudgetPlanConfig(BudgetPlanConfig budgetPlanConfig);
    Task<Period> GetLastFinalizedPeriod();
    Task<Period> GetActiveForecastPeriod();
}
