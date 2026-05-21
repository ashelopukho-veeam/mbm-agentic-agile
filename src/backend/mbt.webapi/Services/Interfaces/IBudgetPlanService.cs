using System.Threading.Tasks;

namespace mbt.webapi.Services.Interfaces;

public interface IBudgetPlanService : IBaseService
{
    Task DraftToSubmitToOwnerStep(string budgetPlanId, string comment, bool notify);
    Task SubmittedToOwnerToPendingApprovalInOriginalForecastStep(string budgetPlanId, string comment, bool notify);
    Task SubmittedToOwnerToDraftInOriginalForecastStep(string budgetPlanId, string comment, bool notify);
    Task SubmittedToOwnerToApprovedInReforecastStep(string budgetPlanId, string comment, bool notify);
    Task SubmittedToOwnerToDraftInReforecastStep(string budgetPlanId, string comment, bool notify);
    Task PendingApprovalToApprovedInOriginalForecastStep(string budgetPlanId, string comment, bool notify);
    Task PendingApprovalToDraftInOriginalForecastStep(string budgetPlanId, string comment, bool notify);
}
