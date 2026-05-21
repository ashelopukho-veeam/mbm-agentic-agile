using mbt.webapi.Domain.Entities;

namespace mbt.webapi;

public static class ErrorMessages
{
    public const string InvalidInput = "Invalid input. Please check your input and try again.";
    public const string UnauthorizedAccess = "Unauthorized access. You do not have permission to perform this action.";
    public const string NotFound = "The requested resource was not found.";

    public const string ReforecastsAreNotAllowed = "Reforecasts are not allowed.";
    public const string BudgetIsNotInDraftStatus = "The budget plan isn't in the 'Draft' status";
    public const string CreatingTransfersIsNotAllowed = "Creating transfers is not allowed";
    public const string TransferCancelWrongStatus = "Unable to cancel a transfer that is in the approved status";

    public static readonly string CopyBudgetInvalidStatus =
        $"Status should be {BudgetStatus.InProgress} or {BudgetStatus.ApprovedPlaceholder}";

    public const string BudgetEditWrongStatus = "Edit allowed only for budget structures in 'InProgress' status";
    public const string GroupedActivityNotFound = "Grouped activity not found";

    public static string BudgetPlanNotFound(string budgetPlanId) => $"Budget plan not found. Id: {budgetPlanId}";


    public static string BudgetNotFound(string budgetId) => $"Budget not found. Id: {budgetId}";

    public const string InvalidBudgetPlanStatus = "Invalid budget plan status";

    public const string IncrementalFundExpireWrongStatus =
        "Unable to expire a incremental fund that is not in the pending approval status";

    public const string IncrementalFundNotFound = "Incremental fund not found";

    public const string IncrementalFundCancelWrongStatus =
        "Cancel available only for Incremental Funds in Draft/Pending Approval status";

    public static readonly string TransferNotFound = "Transfer not found";

    public static readonly string TransferExpireWrongStatus =
        "Unable to expire a transfer that is not in the pending approval status";

    public const string NotPossibleToDeactivateTransfersAndReforecasts =
        "Not possible to deactivate. You have unresolved transfers or incremental funds";

    public static string DuplicateBudgetTitle(string title) => $"A budget with the title '{title}' already exists.";

    public const string NotAllPlansApproved = "You'll have to approve all lines first.";

    public static string PaidMediaTeamApproverNotSet(string team) =>
        $"Paid Media Approver for selected Team should be set up. Team: '{team}'";

    public const string InvalidQuarter = $"Invalid quarter. Valid values are Q1, Q2, Q3, Q4";

    public const string PeriodHasUnprocessedTransfersOrIncFunds =
        "You have unresolved transfers or incremental funds.";

    public const string TransfersBetweenPaidMediaBudgetsMustContainPaidMediaValues =
        "Transfers between paid media budgets must have paid media data";

    public const string TransferFromAndToBudgetsAreTheSame = "The source and destination budgets are the same.";
    public const string GroupedActivityWithSameTitleAlreadyExists = "Grouped activity with same title already exists";

    public const string BudgetStructureNotInProgressOrApprovedPlaceholder =
        "Budget structure is not in progress or approved placeholder";

    public static string ItemNotFound(string item) => $"The {item} was not found.";

    public static string BudgetHasUnprocessedTransfersOrIncFunds(string budgetTitle)
    {
        return $"Budget \"{budgetTitle}\" has unprocessed Transfer or Incremental Fund records.";
    }

    public static string BudgetIsNotApproved(string budgetTitle)
    {
        return $"Budget \"{budgetTitle}\" is not in the approved state.";
    }

    public static string BudgetIsNotApprovedOrApprovedPlaceHolder(string budgetTitle)
    {
        return $"Budget \"{budgetTitle}\" is not in the approved or approved placeholder state.";
    }

    public static string BudgetIsNotInactive(string budgetTitle)
    {
        return $"Budget \"{budgetTitle}\" is not in the inactive state.";
    }

    public static string CannotFindCurrencyRateFor(string currency)
    {
        return "Can't find currency rate for: " + currency;
    }

    public static string BudgetDeactivationIsProhibited(int budgetYear)
    {
        return $"Budget deactivation is prohibited. Budget year: {budgetYear}";
    }
}
