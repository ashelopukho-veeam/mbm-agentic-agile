using JetBrains.Annotations;

namespace mbt.webapi.Endpoints.GroupedActivities;

[PublicAPI]
public class BudgetPlanGroupedActivitySummary
{
    public string PlanId { get; set; }
    public double NetPlannedAmountQ1 { get; set; }
    public double NetPlannedAmountQ2 { get; set; }
    public double NetPlannedAmountQ3 { get; set; }
    public double NetPlannedAmountQ4 { get; set; }
}
