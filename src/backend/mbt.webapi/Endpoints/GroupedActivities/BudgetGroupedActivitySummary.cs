using System.Collections.Generic;
using JetBrains.Annotations;

namespace mbt.webapi.Endpoints.GroupedActivities;

[PublicAPI]
public class BudgetGroupedActivitySummary
{
    public string BudgetId { get; set; }
    public List<BudgetPlanGroupedActivitySummary> Plans { get; set; }
}
