using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using mbt.webapi.Jobs;
using mbt.webapi.WF.Steps;
using WorkflowCore.Interface;
using Budget = mbt.webapi.Domain.Entities.Budget;
using MbtTaskStatus = mbt.webapi.Domain.Entities.MbtTaskStatus;
using TransferExpanded = mbt.webapi.Domain.Entities.TransferExpanded;
using UserProfile = mbt.webapi.Domain.Entities.UserProfile;

namespace mbt.webapi.WF;

[UsedImplicitly]
public class SubmitTransferWorkflow : IWorkflow<WFData>
{
    public string Id => WorkflowNames.SubmitTransferWorkflow;
    public int Version => 3;

    public void Build(IWorkflowBuilder<WFData> builder)
    {
        Action<IWorkflowBuilder<WFData>> fromBudgetTaskAction = t =>
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
                .Input(step => step.TaskId, data => data.FromBudgetTaskId)
                .Input(step => step.TransferId, data => data.TransferId);

        Action<IWorkflowBuilder<WFData>> toBudgetTaskAction = t =>
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
                .Input(step => step.TaskId, data => data.ToBudgetTaskId)
                .Input(step => step.TransferId, data => data.TransferId);


        Action<IWorkflowBuilder<WFData>> approvedTransferAction = t =>
            t.StartWith<ApproveTransferStep>()
                .Input(s => s.TransferId, data => data.TransferId)
                .Then<SendEmailStep>()
                .Input(s => s.To, data => data.GetApprovedTransferNotificationEmailsList())
                .Input(s => s.Subject, data => "Transfer is Approved")
                .Input(s => s.Body,
                    data => MailTemplates.GetTransferApprovedEmail(data.Transfer.Title, data.Transfer.FromBudget.Title,
                        data.Transfer.ToBudget.Title, data.Transfer.Amount.ToString(CultureInfo.InvariantCulture)));

        Action<IWorkflowBuilder<WFData>> rejectedDiffGeoAction = t =>
            t.StartWith<CancelActiveTasksStep>()
                .Input(s => s.AssociatedItemId, data => data.Transfer.Id);

        builder
            .StartWith<ResolveUserStep>()
            .Input(step => step.UserId, data => data.GetTransferApproverUserId(data.Transfer.FromBudget))
            .Output(data => data.FromBudgetApprover, step => step.User)
            .Then<ResolveUserStep>()
            .Input(step => step.UserId, data => data.GetTransferApproverUserId(data.Transfer.ToBudget))
            .Output(data => data.ToBudgetApprover, step => step.User)
            .Parallel()
            .Do(fromBudgetTaskAction)
            .Do(toBudgetTaskAction)
            .Join()
            .If(d => d.FromBudgetApproveStatus == MbtTaskStatus.Approved &&
                     d.ToBudgetApproveStatus == MbtTaskStatus.Approved)
            .Do(approvedTransferAction)
            .If(d => d.IsRejected || d.IsRejectedToInitiator)
            .Do(rejectedDiffGeoAction);
    }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class WFData
{
    public string HostUri { get; set; }

    public string TransferId { get; set; }
    public bool IsSameGeo { get; set; }

    public TransferExpanded Transfer { get; set; }

    public string RequestorEmail { get; set; }

    public string FromBudgetTaskId { get; set; }
    public UserProfile FromBudgetApprover { get; set; }
    public string FromBudgetApproveStatus { get; set; }
    public string ToBudgetTaskId { get; set; }
    public UserProfile ToBudgetApprover { get; set; }
    public string ToBudgetApproveStatus { get; set; }


    public bool IsRejected =>
        FromBudgetApproveStatus == MbtTaskStatus.Rejected ||
        ToBudgetApproveStatus == MbtTaskStatus.Rejected ||
        FromBudgetApproveStatus == MbtTaskStatus.Rejected ||
        ToBudgetApproveStatus == MbtTaskStatus.Rejected;

    public bool IsRejectedToInitiator =>
        FromBudgetApproveStatus == MbtTaskStatus.SendBackToEdit ||
        ToBudgetApproveStatus == MbtTaskStatus.SendBackToEdit;


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
