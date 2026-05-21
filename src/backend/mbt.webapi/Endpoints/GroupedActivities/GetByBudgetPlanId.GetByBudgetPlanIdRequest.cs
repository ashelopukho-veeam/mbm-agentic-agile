using JetBrains.Annotations;

namespace mbt.webapi.Endpoints.GroupedActivities;

[PublicAPI]
public class GetByBudgetPlanIdRequest
{
    public string BudgetPlanId { get; set; }
}
