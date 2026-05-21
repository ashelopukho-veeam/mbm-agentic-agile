namespace mbt.webapi.Domain.Entities;

public static class IncrementalFundStatus
{
    public const string Draft = "Draft";
    public const string Canceled = "Canceled";
    public const string WaitingManagersApprove = "Waiting for Managers Approve";
    public const string WaitingOwnersApprove = "Waiting for Owners Approve";
    public const string Rejected = "Rejected";
    public const string Approved = "Approved";
    public const string PendingApproval = "Pending Approval";
}
