namespace mbt.webapi.Domain.Entities;

public static class BudgetPlanStatusAction
{
    public const string Reject = "Reject";
    public const string ReturnToDraft = "ReturnToDraft";
    public const string SubmitToOwner = "SubmitToOwner";
    public const string SubmitToFinalApproval = "SubmitToFinalApproval";
    public const string Approve = "Approve";
}
