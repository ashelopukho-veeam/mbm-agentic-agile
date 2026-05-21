using System.Collections.Generic;
using System.Globalization;
using System.Text;
using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using mbt.webapi.Jobs;
using mbt.webapi.WF.IncrementalFunds.Steps;
using mbt.webapi.WF.Steps;
using WorkflowCore.Interface;
using MbtTaskStatus = mbt.webapi.Domain.Entities.MbtTaskStatus;
using TaskResultStep = mbt.webapi.WF.IncrementalFunds.Steps.TaskResultStep;

namespace mbt.webapi.WF.IncrementalFunds;

[PublicAPI]
public class IncrementalFundsApproveWorkflow : IWorkflow<IncrementalFundsApproveWorkflowData>
{
    public string Id => WorkflowNames.IncrementalFundsApproveWorkflow;
    public int Version => 1;

    public IStepBuilder<IncrementalFundsApproveWorkflowData, TaskResultStep> GlobalApproveTaskAction(
        IWorkflowBuilder<IncrementalFundsApproveWorkflowData> builder)
    {
        return builder.StartWith<AddTaskStep>()
            .Input(step => step.AssociatedItemId, data => data.IncrementalFundItem.Id)
            .Input(step => step.Message,
                data =>
                    $@"+${data.IncrementalFundItem.Amount} (Q{data.IncrementalFundItem.ToQuarter}) {data.IncrementalFundItem.ToBudget.Title}")
            .Input(step => step.Details, data => GetTaskDetailsInfo(data))
            .Input(step => step.AssignedTo, data => data.GlobalApproverId)
            .Input(step => step.TaskType, _ => TaskTypes.IncrementalFund)
            .Output(data => data.GlobalApproverTaskId, step => step.TaskId)
            .Then<SendEmailStep>()
            .Input(s => s.To, data => new List<string> { data.GlobalApproverId })
            .Input(s => s.Subject, data => GroupedMailTemplates.IncrementalFunds.IncrementalFundSubmitted.Subject)
            .Input(s => s.Body, data =>
                MailTemplates.IncrementalFundSubmitted(data.IncrementalFundItem.ToBudget.Title,
                    data.IncrementalFundItem.Amount.ToString(CultureInfo.InvariantCulture)))
            .WaitFor(WorkflowEventNames.ResolveTask,
                data => data.GlobalApproverTaskId, cancelCondition: data => data.IsRejected || data.IsSentBack)
            .Output(data => data.GlobalApproverTaskStatus, step => step.EventData)
            .Then<TaskResultStep>()
            .Input(t => t.TaskId, data => data.GlobalApproverTaskId);
    }

    public IStepBuilder<IncrementalFundsApproveWorkflowData, TaskResultStep> OwnerTaskAction(
        IWorkflowBuilder<IncrementalFundsApproveWorkflowData> builder)
    {
        return builder
            .StartWith<AddTaskStep>()
            .Input(step => step.AssociatedItemId, data => data.IncrementalFundItem.Id)
            .Input(step => step.Message,
                data =>
                    $@"+${data.IncrementalFundItem.Amount} (Q{data.IncrementalFundItem.ToQuarter}) {data.IncrementalFundItem.ToBudget.Title}")
            .Input(step => step.Details, data => GetTaskDetailsInfo(data))
            .Input(step => step.AssignedTo, data => data.BudgetOwnerId)
            .Input(step => step.TaskType, _ => TaskTypes.IncrementalFund)
            .Output(data => data.OwnerTaskId, step => step.TaskId)
            .Then<SendEmailStep>()
            .Input(s => s.To, data => new List<string> { data.BudgetOwnerId })
            .Input(s => s.Subject, data => GroupedMailTemplates.IncrementalFunds.IncrementalFundSubmitted.Subject)
            .Input(s => s.Body, data =>
                MailTemplates.IncrementalFundSubmitted(data.IncrementalFundItem.ToBudget.Title,
                    data.IncrementalFundItem.Amount.ToString(CultureInfo.InvariantCulture)))
            .WaitFor(WorkflowEventNames.ResolveTask,
                data => data.OwnerTaskId, cancelCondition: data => data.IsRejected || data.IsSentBack)
            .Output(data => data.OwnerTaskStatus, step => step.EventData)
            .Then<TaskResultStep>()
            .Input(t => t.TaskId, data => data.OwnerTaskId);
    }


    public void Build(IWorkflowBuilder<IncrementalFundsApproveWorkflowData> builder)
    {
        builder.Parallel()
            .Do(_ => GlobalApproveTaskAction(builder))
            .Do(_ => OwnerTaskAction(builder))
            .Join()
            .If(d => d.OwnerTaskStatus == MbtTaskStatus.Approved &&
                     d.GlobalApproverTaskStatus == MbtTaskStatus.Approved)
            .Do(t =>
                t.StartWith<ApproveIncrementalFundStep>()
                    .Input(d => d.IncrementalFundId, wfData => wfData.IncrementalFundItem.Id))
            .If(d => d.IsRejected || d.IsSentBack)
            .Do(t =>
                t.StartWith<CancelActiveTasksStep>()
                    .Input(step => step.AssociatedItemId, d => d.IncrementalFundItem.Id))
            .If(d => d.IsRejected)
            .Do(t =>
                t.StartWith<RejectIncrementalFundStep>())
            .If(d => d.IsSentBack)
            .Do(t =>
                t.StartWith<SendBackIncrementalFundStep>());
    }

    private string GetTaskDetailsInfo(IncrementalFundsApproveWorkflowData workflowData)
    {
        var sb = new StringBuilder();
        sb.AppendLine(
            $"To: {workflowData.IncrementalFundItem.ToBudget.Title} (Q{workflowData.IncrementalFundItem.ToQuarter})");
        sb.AppendLine($"Amount: ${workflowData.IncrementalFundItem.Amount}");
        sb.AppendLine($"Requested by: {workflowData.IncrementalFundItem.CreatedByUser.DisplayName}");
        sb.AppendLine($"Reason: {workflowData.IncrementalFundItem.Title}");
        sb.AppendLine($"Description: {workflowData.IncrementalFundItem.Description}");

        return sb.ToString();
    }
}
