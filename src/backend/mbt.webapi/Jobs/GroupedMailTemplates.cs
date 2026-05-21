    using System.Collections.Generic;
using System.Linq;

namespace mbt.webapi.Jobs;

public static class GroupedMailTemplates
{
    public const string BodySeparator = "<br />";
    public const string BodyTag = "##body##";
    public const string BudgetPlanListPageUrlTag = "##budgetPlanListPageUrl##";
    public const string TasksListPageUrlTag = "##tasksListPageUrl##";

    public static class BudgetPlanTemplates
    {
        public static class OriginalForecast
        {
            public static readonly MailTemplate SubmitToOwner =
                new($"Budget Plans Submitted for Your Review",
                    $"""<p>Budget Plans Submitted for Your Review:</p><p><a href='{BudgetPlanListPageUrlTag}'>Link to Budget Plans listing page</a></p><p>{BodyTag}</p>"""
                );

            public static readonly MailTemplate SubmittedToFinalApproval =
                new($"Budget Plans Submitted for Final Approval",
                    $"""<p>Budget Plans Submitted for Final Approval:</p><p><a href='{BudgetPlanListPageUrlTag}'>Link to Budget Plans listing page</a></p><p>{BodyTag}</p>"""
                );

            public static readonly MailTemplate Rejected =
                new($"Budget Plans are not Approved and Require Correction",
                    $"""
                     <p>Budget Plans are not Approved and Require Correction:</p>
                     <p><a href='{BudgetPlanListPageUrlTag}'>Link to Budget Plan listing page</a></p>
                     <p>{BodyTag}</p>
                     """
                );

            public static readonly MailTemplate Approved =
                new($"Budget Plans are Approved",
                    $"""<p>Budget Plans are Approved:</p><p><a href='{BudgetPlanListPageUrlTag}'>Link to Budget Plans listing page</a></p><p>{BodyTag}</p>"""
                );
        }

        public static class Reforecast
        {
            public static readonly MailTemplate SubmitToOwner =
                new($"Reforecasts Submitted for Your Review",
                    $"""<p>Reforecasts Submitted for Your Review:</p><p><a href='{BudgetPlanListPageUrlTag}'>Link to Budget Plans listing page</a></p><p>{BodyTag}</p>"""
                );

            public static readonly MailTemplate Rejected =
                new($"Reforecasts are not Approved and Require Correction",
                    $"""
                     <p>Reforecasts are not Approved and Require Correction:</p>
                     <p><a href='{BudgetPlanListPageUrlTag}'>Link to Budget Plan listing page</a></p>
                     <p>{BodyTag}</p>
                     """
                );

            public static readonly MailTemplate Approved =
                new($"Reforecasts are approved",
                    $"""<p>Reforecasts are approved:</p><p><a href='{BudgetPlanListPageUrlTag}'>Link to Budget Plans listing page</a></p><p>{BodyTag}</p>"""
                );
        }
    }

    public static class IncrementalFunds
    {
        public static readonly MailTemplate Approved = new("Incremental Funds are Approved",
            $"""<p>Incremental Funds are Approved:</p><p>{BodyTag}</p>"""
        );

        public static readonly MailTemplate SentBackToEdit = new("Incremental Funds: Change is Requested",
            $"""<p>Incremental Funds: Change is Requested:</p><p>{BodyTag}</p>"""
        );

        public static readonly MailTemplate Rejected = new("Incremental Funds are Rejected",
            $"""<p>Incremental Funds are Rejected:</p><p>{BodyTag}</p>"""
        );

        public static readonly MailTemplate Expired = new("Budget Incremental Funds are Expired",
            $"""
             <p>Budget Incremental Funds are Expired:</p>
             <p>{BodyTag}</p>
             """
        );

        public static readonly MailTemplate IncrementalFundSubmitted = new("Incremental Funds Approval Request",
            $"""
              <p>The following Incremental Funds are awaiting your approval:</p>
              {BodyTag}
              <p>Follow this <a href='{TasksListPageUrlTag}'>link</a> to approve or reject.<p>
              """);
    }

    public static class Transfers
    {
        public static readonly MailTemplate Approved = new("Budget Transfers are Approved",
            $"<p>Budget Transfers are Approved:</p><p>{BodyTag}</p>");

        public static readonly MailTemplate Rejected = new("Budget Transfers are Rejected",
            $"<p>Budget Transfers are Rejected:</p><p>{BodyTag}</p>");

        public static readonly MailTemplate SentBackToEdit = new("Budget Transfers: Change is Requested",
            $"<p>Budget Transfers: Change is Requested:</p><p>{BodyTag}</p>");

        public static readonly MailTemplate Expired = new("Budget Transfers are Expired",
            $"<p>Budget Transfers are Expired:</p><p>{BodyTag}</p>");

        public static readonly MailTemplate TransferSubmitted = new MailTemplate("Budget Transfers Approval Request",
            $"""
                <p>The following transfers are awaiting your approval:</p>
                {BodyTag}
                <p>Follow this <a href='{TasksListPageUrlTag}'>link</a> to approve or reject.<p>
            """);
    }

    public static class Chat
    {
        public static readonly MailTemplate UserMentioned = new("You have been mentioned in MBM chat",
            $"<p>You have been mentioned in MBM chat</p><p>{BodyTag}</p>");
    }

    private static List<MailTemplate> Templates { get; } = new()
    {
        BudgetPlanTemplates.OriginalForecast.SubmitToOwner,
        BudgetPlanTemplates.OriginalForecast.SubmittedToFinalApproval,
        BudgetPlanTemplates.OriginalForecast.Rejected,
        BudgetPlanTemplates.OriginalForecast.Approved,
        //
        BudgetPlanTemplates.Reforecast.SubmitToOwner,
        BudgetPlanTemplates.Reforecast.Rejected,
        BudgetPlanTemplates.Reforecast.Approved,
        //
        IncrementalFunds.Approved,
        IncrementalFunds.SentBackToEdit,
        IncrementalFunds.Rejected,
        IncrementalFunds.Expired,
        IncrementalFunds.IncrementalFundSubmitted,
        //
        Transfers.Approved,
        Transfers.Rejected,
        Transfers.SentBackToEdit,
        Transfers.Expired,
        Transfers.TransferSubmitted,
        //
        Chat.UserMentioned
    };

    public static MailTemplate GetMailTemplate(string group)
    {
        return Templates.FirstOrDefault(t => t.Subject == group);
    }
}

public record MailTemplate(string Subject, string Body);
