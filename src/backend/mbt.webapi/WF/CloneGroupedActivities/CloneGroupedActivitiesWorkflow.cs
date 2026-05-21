using JetBrains.Annotations;
using WorkflowCore.Interface;

namespace mbt.webapi.WF.CloneGroupedActivities;

[PublicAPI]
public class CloneGroupedActivitiesWorkflow : IWorkflow<CloneGroupedActivitiesWorkflowData>
{
    public const string WorkflowName = "CloneGroupedActivitiesWorkflow";

    public string Id => WorkflowName;
    public int Version => 1;


    public void Build(IWorkflowBuilder<CloneGroupedActivitiesWorkflowData> builder)
    {
        builder.ForEach(d => d.PlansIds, data => false)
            .Do(x =>
                x.StartWith<CloneGroupedActivitiesStep>()
                    .Input(s => s.PlanId, (data, context) => context.Item.ToString()));
    }
}
