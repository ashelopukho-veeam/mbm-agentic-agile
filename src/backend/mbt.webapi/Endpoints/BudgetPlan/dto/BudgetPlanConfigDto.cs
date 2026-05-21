using JetBrains.Annotations;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Shared;


namespace mbt.webapi.Endpoints.BudgetPlan.dto;

[PublicAPI]
public class BudgetPlanConfigDto
{
    public Period ActiveDraftForecast { get; set; }

    public Period LastFinalizedForecast { get; set; }

    public static BudgetPlanConfigDto FromBudgetPlanConfig(BudgetPlanConfig budgetPlanConfig)
    {
        var activeDraftForecast = new Period(budgetPlanConfig.CurrentBudgetPlanYear, budgetPlanConfig.CurrentBudgetPlanName);
        var lastFinalizedForecast = activeDraftForecast.Previous();

        BudgetPlanConfigDto dto = new()
        {
            ActiveDraftForecast = activeDraftForecast,
            LastFinalizedForecast = lastFinalizedForecast
        };

        return dto;
    }
}
