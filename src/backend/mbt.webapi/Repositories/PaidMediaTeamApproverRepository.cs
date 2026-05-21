using mbt.webapi.Domain.Entities;
using mbt.webapi.Services.Interfaces;

namespace mbt.webapi.Repositories;

public interface IPaidMediaTeamApproverRepository : IDbBaseRepositoryExpandable<PaidMediaTeamApprover, PaidMediaTeamApproverExpanded>
{
}

public class PaidMediaTeamApproverRepository : DbBaseRepositoryExpandable<PaidMediaTeamApprover, PaidMediaTeamApproverExpanded>,
    IPaidMediaTeamApproverRepository
{
    public PaidMediaTeamApproverRepository(IDbContext context, ICurrentUserContext currentUserContext) : base(context,
        currentUserContext)
    {
        ExpandLookupsMapping = LookupMappings.PaidMediaTeamApproverExpandedMapping;
    }
}
