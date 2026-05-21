using System.Collections.Generic;

namespace mbt.webapi.Endpoints.BudgetPlan.dto;

public class BulkBudgetPlanStatusData
{
    public List<string> BudgetPlanIds { get; set; }

    public string Status { get; set; }
    public string Comment { get; set; }

    public bool Notify { get; set; }

    public string Quarter { get; set; }
}
