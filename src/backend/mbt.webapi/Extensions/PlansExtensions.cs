using System.Collections.Generic;
using System.Linq;
using mbt.webapi.Domain.Entities;

namespace mbt.webapi.Extensions;

public static class PlansExtensions
{
    public static List<BudgetPlan> GetActivePlans(this List<BudgetPlan> plans)
    {
        // index of first in not final status
        var firstNotFinalIndex = plans.FindIndex(p => p.Status != BudgetPlanStatus.Final);

        return firstNotFinalIndex == -1 ? plans : plans.Take(firstNotFinalIndex + 1).ToList();
    }

    public static BudgetPlan GetActivePlanByQuarter(this List<BudgetPlan> plans, string quarter)
    {
        return GetActivePlans(plans).FirstOrDefault(p => p.Quarter == quarter);
    }
}
