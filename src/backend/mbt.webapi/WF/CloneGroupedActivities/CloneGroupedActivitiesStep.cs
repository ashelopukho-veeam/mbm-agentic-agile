using System.Threading.Tasks;
using mbt.webapi.Services.Interfaces;
using mbt.webapi.Utils;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace mbt.webapi.WF.CloneGroupedActivities;

public class CloneGroupedActivitiesStep : StepBodyAsync
{
    public string PlanId { get; set; }

    private readonly IGroupActivitiesService _groupActivitiesService;
    private readonly IBudgetService _budgetService;

    public CloneGroupedActivitiesStep(IGroupActivitiesService groupActivitiesService, IBudgetService budgetService)
    {
        _groupActivitiesService = groupActivitiesService;
        _budgetService = budgetService;
    }

    public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        var budget = await _budgetService.GetBudgetByPlanId(PlanId);
        var fromPlan = budget.GetBudgetPlanById(PlanId);

        var nextQuarter = QuarterUtils.GetNextQuarterName(fromPlan.Quarter);
        if (nextQuarter != null)
        {
            var toPlanId = budget.GetPlanByQuarter(nextQuarter).Id;
            var groupedActivities = await _groupActivitiesService.GetByBudgetPlanId(PlanId);
            foreach (var ga in groupedActivities)
            {
                await _groupActivitiesService.Clone(ga, toPlanId);
            }
        }


        return ExecutionResult.Next();
    }
}
