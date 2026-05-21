using System.Collections.Generic;

namespace mbt.webapi.BuiltIn;

public static class ChatSystemMessages
{
    public static readonly string UserTag = "#user";
    public static readonly string DateTimeTag = "#datetime";

    public static readonly string BudgetPlanSubmittedToOwner = $"Submitted to Owner by {UserTag}";
    public static readonly string BudgetPlanSubmittedToFinalApprove = $"Submitted for final approval by {UserTag}";
    public static readonly string BudgetPlanReturnedToDraft = $"Returned to Draft by {UserTag}";
    public static readonly string BudgetPlanApproved = $"Approved by {UserTag}";
    public static readonly string BudgetPlanRejected = $"Rejected by {UserTag}";

    public static readonly string TransferRejected = "Transfer is rejected by ";
    public static readonly string TransferRejectedToInitiator = "Transfer is sent back for edit by ";

    public static readonly string TransferSubmited = "Transfer is submitted by ";
    public static readonly string TransferSubmitCanceled = "Transfer is canceled by ";

    public static readonly string ItemRejected = "{0} is rejected by {1}";
    public static readonly string ItemRejectedToInitiator = "{0} is sent back for edit by {1}";
    public static readonly string ItemApproved = "{0} is approved by {1}";


    public static readonly string ItemSubmitted = "{0} is submitted by {1}";
    public static readonly string ItemSubmitCanceled = "{0} is canceled by {1}";

    public static readonly string TaskApproved = "Task is approved by ";
    public static readonly string TransferExpired = "Transfer marked as expired by ";
    public const string IncrementalFundExpired = "Incremental fund marked as expired by ";

    public static string ReplaceTags(string msg, Dictionary<string, string> tags)
    {
        var resultMsg = msg;
        foreach (var t in tags) resultMsg = resultMsg.Replace(t.Key, t.Value);
        return resultMsg;
    }
}
