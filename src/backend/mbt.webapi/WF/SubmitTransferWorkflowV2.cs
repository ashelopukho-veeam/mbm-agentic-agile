using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Jobs;
using mbt.webapi.WF.Steps;
using WorkflowCore.Interface;

namespace mbt.webapi.WF;

[UsedImplicitly]
public class SubmitTransferWorkflowV2 : IWorkflow<SubmitTransferWorkflowDataV2>
{
    public string Id => WorkflowNames.SubmitTransferWorkflow;
    public int Version => 4;

    private readonly Action<IWorkflowBuilder<SubmitTransferWorkflowDataV2>> _paidMediaTeamApprove = builder =>
    {
        builder
            .StartWith<AddTaskStep>()
            .Input(step => step.AssociatedItemId, data => data.Transfer.Id)
            .Input(step => step.Message,
                data =>
                    $@"${data.Transfer.Amount} Transfer - Paid Media validation")
            .Input(step => step.Details, data => data.GetTransferTaskDetails())
            .Input(step => step.AssignedTo, data => data.PaidMediaTeamApproverId)
            .Input(step => step.TaskType, _ => TaskTypes.Transfer)
            .Output(data => data.PaidMediaTeamApproverTaskId, step => step.TaskId)
            .Then<SendEmailStep>()
            .Input(s => s.To, data => new List<string> { data.PaidMediaTeamApproverId })
            .Input(s => s.Subject, data => GroupedMailTemplates.Transfers.TransferSubmitted.Subject)
            .Input(s => s.Body, data =>
                MailTemplates.TransferSubmitted(
                    data.Transfer.FromBudget.Title,
                    data.Transfer.ToBudget.Title,
                    data.Transfer.Amount.ToString(CultureInfo.InvariantCulture)))
            .WaitFor(WorkflowEventNames.ResolveTask,
                data => data.PaidMediaTeamApproverTaskId,
                cancelCondition: data => data.IsRejected || data.IsRejectedToInitiator)
            .Output(data => data.PaidMediaTeamApproverTaskStatus, step => step.EventData)
            .Then<TaskResultStep>()
            .Input(t => t.TaskId, data => data.PaidMediaTeamApproverTaskId);
    };

    private readonly Action<IWorkflowBuilder<SubmitTransferWorkflowDataV2>> _fromBudgetTaskAction = t =>
        t.StartWith<AddTaskStep>()
            .Input(step => step.AssociatedItemId, data => data.TransferId)
            .Input(step => step.Message,
                data =>
                    data.GetTransferTaskTitle(data.Transfer.FromBudget.Title,
                        data.Transfer.FromQuarter, "-$" + data.Transfer.Amount))
            .Input(step => step.Details, data => data.GetTransferTaskDetails())
            .Input(step => step.AssignedTo, data => data.FromBudgetApprover.Id)
            .Input(step => step.TaskType, _ => TaskTypes.Transfer)
            .Output(data => data.FromBudgetTaskId, step => step.TaskId)
            .Then<SendEmailStep>()
            .Input(s => s.To, data => new List<string> { data.FromBudgetApprover.Email })
            .Input(s => s.Subject, data => GroupedMailTemplates.Transfers.TransferSubmitted.Subject)
            .Input(s => s.Body, data =>
                MailTemplates.TransferSubmitted(data.Transfer.FromBudget.Title,
                    data.Transfer.ToBudget.Title,
                    data.Transfer.Amount.ToString(CultureInfo.InvariantCulture)))
            .WaitFor(WorkflowEventNames.ResolveTask, data => data.FromBudgetTaskId, null,
                d => d.IsRejected || d.IsRejectedToInitiator)
            .Output(data => data.FromBudgetApproveStatus, step => step.EventData)
            .Then<TaskResultStep>()
            .Input(step => step.TaskId, data => data.FromBudgetTaskId);


    private readonly Action<IWorkflowBuilder<SubmitTransferWorkflowDataV2>> _approvedTransferAction = t =>
        t.StartWith<ApproveTransferStep>()
            .Input(s => s.TransferId, data => data.TransferId)
            .Then<SendEmailStep>()
            .Input(s => s.To, data => data.GetApprovedTransferNotificationEmailsList())
            .Input(s => s.Subject, data => GroupedMailTemplates.Transfers.Approved.Subject)
            .Input(s => s.Body,
                data => MailTemplates.GetTransferApprovedEmail(data.Transfer.Title, data.Transfer.FromBudget.Title,
                    data.Transfer.ToBudget.Title, data.Transfer.Amount.ToString(CultureInfo.InvariantCulture)));


    private readonly Action<IWorkflowBuilder<SubmitTransferWorkflowDataV2>> _toBudgetTaskAction = t =>
        t.StartWith<AddTaskStep>()
            .Input(step => step.AssociatedItemId, data => data.TransferId)
            .Input(step => step.Message,
                data => data.GetTransferTaskTitle(data.Transfer.ToBudget.Title, data.Transfer.ToQuarter,
                    "+$" + data.Transfer.Amount))
            .Input(step => step.Details, data => data.GetTransferTaskDetails())
            .Input(step => step.AssignedTo, data => data.ToBudgetApprover.Id)
            .Input(step => step.TaskType, _ => TaskTypes.Transfer)
            .Output(data => data.ToBudgetTaskId, step => step.TaskId)
            .Then<SendEmailStep>()
            .Input(s => s.To, data => new List<string> { data.ToBudgetApprover.Email })
            .Input(s => s.Subject, data => GroupedMailTemplates.Transfers.TransferSubmitted.Subject)
            .Input(s => s.Body, data =>
                MailTemplates.TransferSubmitted(data.Transfer.FromBudget.Title,
                    data.Transfer.ToBudget.Title,
                    data.Transfer.Amount.ToString(CultureInfo.InvariantCulture)))
            .WaitFor(WorkflowEventNames.ResolveTask, data => data.ToBudgetTaskId, null,
                d => d.IsRejected || d.IsRejectedToInitiator)
            .Output(data => data.ToBudgetApproveStatus, step => step.EventData)
            .Then<TaskResultStep>()
            .Input(step => step.TaskId, data => data.ToBudgetTaskId);


    private readonly Action<IWorkflowBuilder<SubmitTransferWorkflowDataV2>> _rejectTransferAction = t =>
        t.StartWith<CancelActiveTasksStep>()
            .Input(s => s.AssociatedItemId, data => data.Transfer.Id);

    public void Build(IWorkflowBuilder<SubmitTransferWorkflowDataV2> builder)
    {
        builder
            .If(data => !string.IsNullOrEmpty(data.PaidMediaTeamApproverId))
            .Do(_paidMediaTeamApprove)
            .If(data => !data.IsRejected && !data.IsRejectedToInitiator)
            .Do(b =>
                b
                    .StartWith<ResolveUserStep>()
                    .Input(step => step.UserId, data => data.GetTransferApproverUserId(data.Transfer.FromBudget))
                    .Output(data => data.FromBudgetApprover, step => step.User)
                    .Then<ResolveUserStep>()
                    .Input(step => step.UserId, data => data.GetTransferApproverUserId(data.Transfer.ToBudget))
                    .Output(data => data.ToBudgetApprover, step => step.User)
                    .Parallel()
                    .Do(_fromBudgetTaskAction)
                    .Do(_toBudgetTaskAction)
                    .Join()
                    .If(d => d.FromBudgetApproveStatus == MbtTaskStatus.Approved &&
                             d.ToBudgetApproveStatus == MbtTaskStatus.Approved)
                    .Do(_approvedTransferAction)
                    .If(d => d.IsRejected || d.IsRejectedToInitiator)
                    .Do(_rejectTransferAction));
    }
}

[UsedImplicitly]
public class SubmitTransferWorkflowDataV2
{
    public string HostUri { get; set; }
    public string TransferId { get; set; }

    public TransferExpanded Transfer { get; set; }
    public string RequestorEmail { get; set; }

    public string FromBudgetTaskId { get; set; }
    public UserProfile FromBudgetApprover { get; set; }
    public string FromBudgetApproveStatus { get; set; }
    public string ToBudgetTaskId { get; set; }
    public UserProfile ToBudgetApprover { get; set; }
    public string ToBudgetApproveStatus { get; set; }

    public string PaidMediaTeamApproverId { get; set; }
    public string PaidMediaTeamApproverTaskId { get; set; }
    public string PaidMediaTeamApproverTaskStatus { get; set; }

    public bool IsRejected =>
        FromBudgetApproveStatus == MbtTaskStatus.Rejected ||
        ToBudgetApproveStatus == MbtTaskStatus.Rejected ||
        PaidMediaTeamApproverTaskStatus == MbtTaskStatus.Rejected;

    public bool IsRejectedToInitiator =>
        FromBudgetApproveStatus == MbtTaskStatus.SendBackToEdit ||
        ToBudgetApproveStatus == MbtTaskStatus.SendBackToEdit ||
        PaidMediaTeamApproverTaskStatus == MbtTaskStatus.SendBackToEdit;


    public string GetTransferTaskTitle(string title, string quarter, string amount)
    {
        return $@"{amount} ({quarter.ToUpper()}) {title} ";
    }

    public string GetTransferTaskDetails()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"From: {Transfer.FromBudget.Title} ({Transfer.FromQuarter.ToUpper()})");
        sb.AppendLine($"To: {Transfer.ToBudget.Title} ({Transfer.ToQuarter.ToUpper()})");
        sb.AppendLine($"Amount: ${Transfer.Amount}");
        sb.AppendLine($"Requested by: {Transfer.CreatedByUser.DisplayName}");
        sb.AppendLine($"Reason: {Transfer.Title}");
        sb.AppendLine($"Comment: {Transfer.Comment}");

        return sb.ToString();
    }

    public string GetTaskUri(string taskId)
    {
        return HostUri + $"/tasks/view/{taskId}";
    }

    public string GetTransferApproverUserId(Budget b)
    {
        return Transfer.Amount < 5000 ? b.ManagerId :
            Transfer.Amount is >= 5000 and < 20000 ? b.ParentManagerId :
            b.OwnerId;
    }

    public List<string> GetApprovedTransferNotificationEmailsList()
    {
        var resultList = new List<string> { RequestorEmail };
        switch (Transfer.Amount)
        {
            case >= 5000 and < 20000:
                resultList.Add(Transfer.ToBudget.ManagerId);
                resultList.Add(Transfer.FromBudget.ManagerId);
                break;
            case >= 20000:
                resultList.Add(Transfer.ToBudget.ParentManagerId);
                resultList.Add(Transfer.FromBudget.ParentManagerId);
                resultList.Add(Transfer.ToBudget.ManagerId);
                resultList.Add(Transfer.FromBudget.ManagerId);
                break;
        }

        return resultList;
    }
}
