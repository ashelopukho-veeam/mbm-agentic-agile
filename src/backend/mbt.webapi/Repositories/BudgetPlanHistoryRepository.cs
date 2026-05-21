using mbt.webapi.Domain.Entities;
using mbt.webapi.Services.Interfaces;

namespace mbt.webapi.Repositories;

public interface
    IBudgetPlanHistoryRepository : IDbBaseRepositoryExpandable<BudgetPlanHistoryItem, BudgetPlanHistoryItemExpanded>
{
}

public class BudgetPlanHistoryRepository :
    DbBaseRepositoryExpandable<BudgetPlanHistoryItem, BudgetPlanHistoryItemExpanded>, IBudgetPlanHistoryRepository
{
    public BudgetPlanHistoryRepository(IDbContext context, ICurrentUserContext currentUserContext) : base(context,
        currentUserContext)
    {
    }
}
