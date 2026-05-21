using System.Collections.Generic;
using JetBrains.Annotations;
using TitleNumberValuePair = mbt.webapi.Domain.Entities.TitleNumberValuePair;

namespace mbt.webapi.Endpoints.BudgetStructure.dto;

[PublicAPI]
public class BudgetPlanDto
{
    public string Id { get; set; }

    public string Quarter { get; set; }
    public double Q1 { get; set; }
    public double Q2 { get; set; }
    public double Q3 { get; set; }
    public double Q4 { get; set; }

    public List<TitleNumberValuePair> Segments { get; set; } = new();
    public List<TitleNumberValuePair> Campaigns { get; set; } = new();

    public string Status { get; set; }

    public static BudgetPlanDto FromBudgetPlan(Domain.Entities.BudgetPlan budgetPlan)
    {
        var budgetPlanDto = new BudgetPlanDto()
        {
            Id = budgetPlan.Id,
            Quarter = budgetPlan.Quarter,
            Q1 = budgetPlan.Q1,
            Q2 = budgetPlan.Q2,
            Q3 = budgetPlan.Q3,
            Q4 = budgetPlan.Q4,
            Segments = budgetPlan.Segments,
            Campaigns = budgetPlan.Campaigns,
            Status = budgetPlan.Status
        };

        return budgetPlanDto;
    }
}
