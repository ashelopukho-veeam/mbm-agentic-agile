using System.Collections.Generic;
using System.Threading.Tasks;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using mbt.webapi.Shared;

namespace mbt.webapi.Services;

public class ApiService : IApiService
{
    private readonly IDbBaseRepository<BudgetLevel> _budgetLevelsRepository;
    private readonly IDbBaseRepository<CostCenter> _costCenters;
    private readonly IDbBaseRepository<AppConfig> _appConfigRepository;
    private readonly IDbBaseRepository<CommonConfig> _commonConfigRepository;
    private readonly IDbBaseRepository<BudgetPlanConfig> _budgetPlanConfigRepository;

    public ApiService(
        IDbBaseRepository<AppConfig> appConfigRepository,
        IDbBaseRepository<BudgetLevel> budgetLevelsRepository,
        IDbBaseRepository<CostCenter> costCenters,
        IDbBaseRepository<CommonConfig> commonConfigRepository,
        IDbBaseRepository<BudgetPlanConfig> budgetPlanConfigRepository)
    {
        _appConfigRepository = appConfigRepository;
        _budgetLevelsRepository = budgetLevelsRepository;
        _costCenters = costCenters;
        _commonConfigRepository = commonConfigRepository;
        _budgetPlanConfigRepository = budgetPlanConfigRepository;
    }

    public Task<List<BudgetLevel>> GetBudgetLevels()
    {
        return _budgetLevelsRepository.GetAsync();
    }

    public Task<List<CostCenter>> GetCostCenters()
    {
        return _costCenters.GetAsync();
    }


    public async Task<List<CostCenter>> SetCostCenters(List<CostCenter> costCenters)
    {
        await _costCenters.ClearAsync();

        foreach (var costCenter in costCenters)
        {
            costCenter.Id = null;
            await _costCenters.CreateAsync(costCenter);
        }

        return costCenters;
    }

    public AppConfig GetAppConfig()
    {
        return _appConfigRepository.FindOne(_ => true);
    }

    public Task<AppConfig> GetAppConfigAsync()
    {
        return _appConfigRepository.FindOneAsync(_ => true);
    }


    public async Task SetAppConfigAsync(AppConfig config)
    {
        var currentConfig = await GetAppConfigAsync();
        if (currentConfig == null)
        {
            await _appConfigRepository.CreateAsync(config);
        }
        else
        {
            config.Id = currentConfig.Id;
            await _appConfigRepository.UpdateAsync(config);
        }
    }

    public CommonConfig GetCommonConfig()
    {
        var commonConfig = _commonConfigRepository.FindOne(_ => true);

        return commonConfig ?? new CommonConfig();
    }

    public async Task<CommonConfig> GetCommonConfigAsync()
    {
        var commonConfig = await _commonConfigRepository.FindOneAsync(_ => true);

        return commonConfig ?? new CommonConfig();
    }

    public async Task SetCommonConfigAsync(CommonConfig config
    )
    {
        var currentConfig = await _commonConfigRepository.FindOneAsync(_ => true);
        if (currentConfig != null)
        {
            config.Id = currentConfig.Id;
            await _commonConfigRepository.UpdateAsync(config);
        }
        else
        {
            await _commonConfigRepository.CreateAsync(config);
        }
    }

    public async Task<BudgetPlanConfig> GetBudgetPlanConfig()
    {
        var planConfig = await _budgetPlanConfigRepository.FindOneAsync(_ => true);
        return planConfig;
    }

    public async Task SetBudgetPlanConfig(BudgetPlanConfig budgetPlanConfig)
    {
        var planConfig = await _budgetPlanConfigRepository.FindOneAsync(_ => true);
        if (planConfig != null)
        {
            budgetPlanConfig.Id = planConfig.Id;
            await _budgetPlanConfigRepository.UpdateAsync(budgetPlanConfig);
        }
        else
        {
            await _budgetPlanConfigRepository.CreateAsync(budgetPlanConfig);
        }
    }

    public async Task<Period> GetLastFinalizedPeriod()
    {
        var planConfig = await GetBudgetPlanConfig();
        var period = new Period(planConfig.CurrentBudgetPlanYear, planConfig.CurrentBudgetPlanName);
        var lastFinalizedPeriod = period - 1;

        return lastFinalizedPeriod;
    }

    public async Task<Period> GetActiveForecastPeriod()
    {
        var planConfig = await GetBudgetPlanConfig();
        var period = new Period(planConfig.CurrentBudgetPlanYear, planConfig.CurrentBudgetPlanName);
        return period;
    }
}
