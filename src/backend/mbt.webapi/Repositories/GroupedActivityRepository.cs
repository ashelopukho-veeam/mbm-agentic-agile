using mbt.webapi.Domain.Entities;
using mbt.webapi.Services.Interfaces;

namespace mbt.webapi.Repositories;

public interface IGroupedActivityRepository : IDbBaseRepositoryExpandable<GroupedActivity, GroupedActivityExpanded>
{
}

public class GroupedActivityRepository : DbBaseRepositoryExpandable<GroupedActivity, GroupedActivityExpanded>,
    IGroupedActivityRepository

{
    public GroupedActivityRepository(IDbContext context, ICurrentUserContext currentUserContext) : base(context,
        currentUserContext)
    {
        ExpandLookupsMapping = LookupMappings.GroupedActivityExpanded;
    }
}

