using mbt.webapi.Domain.Entities;
using mbt.webapi.Services.Interfaces;

namespace mbt.webapi.Repositories;

public interface IIncrementalFundsRepository : IDbBaseRepositoryExpandable<IncrementalFund, IncrementalFundExpanded>
{
}

public class IncrementalFundsRepository : DbBaseRepositoryExpandable<IncrementalFund, IncrementalFundExpanded>,
    IIncrementalFundsRepository
{
    public IncrementalFundsRepository(IDbContext context, ICurrentUserContext currentUserContext) : base(context,
        currentUserContext)
    {
        ExpandLookupsMapping = LookupMappings.IncrementalFundsExpanded;
    }
}
