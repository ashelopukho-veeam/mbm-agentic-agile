namespace mbt.webapi.Domain.Entities;

public class BudgetPlanHistoryItemExpanded : BudgetPlanHistoryItem, IBaseItemExpanded
{
    public UserProfile CreatedByUser { get; set; }
    public UserProfile ModifiedByUser { get; set; }
}
