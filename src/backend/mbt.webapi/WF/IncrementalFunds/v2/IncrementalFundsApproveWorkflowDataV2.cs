using JetBrains.Annotations;
using IncrementalFundExpanded = mbt.webapi.Domain.Entities.IncrementalFundExpanded;
using MbtTaskStatus = mbt.webapi.Domain.Entities.MbtTaskStatus;


namespace mbt.webapi.WF.IncrementalFunds.v2;

[PublicAPI]
public class IncrementalFundsApproveWorkflowDataV2
{
    public IncrementalFundExpanded IncrementalFundItem { get; set; }

    public string GlobalApproverTaskId { get; set; }
    public string GlobalApproverTaskStatus { get; set; }
    public string GlobalApproverId { get; set; }

    public string OwnerTaskId { get; set; }
    public string BudgetOwnerId { get; set; }
    public string OwnerTaskStatus { get; set; }

    public string PaidMediaTeamApproverId { get; set; }
    public string PaidMediaTeamApproverTaskId { get; set; }
    public string PaidMediaTeamApproverTaskStatus { get; set; }

    public bool IsRejected =>
        GlobalApproverTaskStatus == MbtTaskStatus.Rejected || OwnerTaskStatus == MbtTaskStatus.Rejected || PaidMediaTeamApproverTaskStatus == MbtTaskStatus.Rejected;

    public bool IsSentBack =>
        GlobalApproverTaskStatus == MbtTaskStatus.SendBackToEdit || OwnerTaskStatus == MbtTaskStatus.SendBackToEdit || PaidMediaTeamApproverTaskStatus == MbtTaskStatus.SendBackToEdit;

}
