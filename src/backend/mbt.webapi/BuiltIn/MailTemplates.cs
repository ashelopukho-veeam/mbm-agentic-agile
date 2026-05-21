namespace mbt.webapi.BuiltIn;

public static class MailTemplates
{
    public static string BudgetPlanRejected(string title, string comment)
    {
        return $"""<p>{title}<br/>{comment}</p>""";
    }

    public static string IncrementalFundSubmitted(string toBudget, string amount)
    {
        return
            $"""
             <p>To: {toBudget}<br/>
             In the amount of: {amount}</p>
             """;
    }

    public static string TransferSubmitted(string fromBudget, string toBudget, string amount)
    {
        return
            $"""
             <p>From: {fromBudget}<br/>
             To: {toBudget}<br/>
             In the amount of: {amount}</p>
             """;
    }

    public static string GetTransferApprovedEmail(string transferTitle, string fromBudget, string toBudget,
        string amount)
    {
        return $"""
                <p>{transferTitle}<p>
                From: {fromBudget}<br/>
                To: {toBudget}<br/>
                In the amount of: {amount}
                """;
    }

    public static string GetTransferRejectedToInitiatorEmail(
        string transferTitle,
        string fromBudget,
        string toBudget,
        string amount,
        string comment,
        string linkToTransfer)
    {
        return $"""
                <p>{transferTitle}<br/>
                From: {fromBudget}<br/>
                To: {toBudget}<br/>
                In the amount of: {amount}
                <p>{comment}</p>
                Make the required corrections here: <a href='{linkToTransfer}'>{linkToTransfer}</a>
                """;
    }

    public static string GetTransferRejectedEmail(string transferTitle,
        string fromBudget, string toBudget, string amount, string comment
    )
    {
        return
            $"""
             <p>{transferTitle}</p>
             From: {fromBudget}<br/>
             To: {toBudget}<br/>
             In the amount of: {amount}
             <p>Comment: {comment}</p>
             """;
    }

    public static string GetChatNotificationEmail(string chatMessage, string chatLink)
    {
        return
            $"""
             <p>Chat message: {chatMessage}</p>
             <p>Click <a href="{chatLink}">here</a> to check the details and add your answer.</p>
             """;
    }

    public static string GetIncrementalFund_Approved_MailBody(string title, string toBudgetName, double amount)
    {
        return
            $"""
             <p>{title}<br/>
             To: {toBudgetName}<br/>
             In the amount of: {amount}
             </p>
             """;
    }

    public static string GetIncrementalFund_Rejected_MailBody(string incrementalFundTitle, string budgetName,
        double amount, string comment)
    {
        return $"""
                <p>{incrementalFundTitle}</p>
                <p>
                To: {budgetName}<br/>
                In the amount of: {amount}<br/>
                Comment: {comment}
                </p>
                """;
    }

    public static string GetIncrementalFund_SendBack_MailBody(
        string incrementalFundTitle,
        string budgetName, double amount, string comment, string editFormLink)
    {
        return
            $"""
             <p>{incrementalFundTitle}</p>
             <p>
             To: {budgetName}<br/>
             In the amount of: {amount}<br/>
             Comment: {comment}<br/>
             </p>
             <p>Make the corrections here: <a href='{editFormLink}'>{editFormLink}</a>
             </p>
             """;
    }


    public static string TransferExpiredBody(string transferDisplayPageUrl,
        string transferTitle,
        string fromBudget, string toBudget,
        string amount)
    {
        return
            $"""
             The following transfer <a href='{transferDisplayPageUrl}'>{transferTitle}</a>
             from: {fromBudget} to: {toBudget} in the amount of {amount} has been expired.
             No further actions are needed.
             """;
    }

    public static string IncrementalFundExpiredBody(
        string incrementalFundTitle, string toBudget, string amount, string displayFormLink)
    {
        return
            $"""
             The following incremental fund <a href='{displayFormLink}'>{incrementalFundTitle}</a>
             to {toBudget} in the amount of {amount} has been expired.
             No further actions are needed.
             """;
    }
}
