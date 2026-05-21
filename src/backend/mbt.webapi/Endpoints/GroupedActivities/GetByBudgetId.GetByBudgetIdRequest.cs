using JetBrains.Annotations;

namespace mbt.webapi.Endpoints.GroupedActivities;

[PublicAPI]
public class GetByBudgetIdRequest
{
    public string BudgetId { get; set; }
}
