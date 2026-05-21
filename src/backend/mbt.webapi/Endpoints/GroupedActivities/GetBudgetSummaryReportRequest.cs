using JetBrains.Annotations;

namespace mbt.webapi.Endpoints.GroupedActivities;

[PublicAPI]
public class GetBudgetSummaryReportRequest
{
    public string BudgetId { get; set; }
}
