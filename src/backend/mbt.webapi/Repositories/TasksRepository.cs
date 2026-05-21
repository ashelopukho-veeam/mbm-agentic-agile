using mbt.webapi.Domain.Entities;
using mbt.webapi.Services.Interfaces;

namespace mbt.webapi.Repositories;

public interface ITasksRepository : IDbBaseRepositoryExpandable<MbtTask, MbtTaskExpanded>
{
}

public class TasksRepository :  DbBaseRepositoryExpandable<MbtTask, MbtTaskExpanded> , ITasksRepository
{
    public TasksRepository(IDbContext context, ICurrentUserContext currentUserContext) : base(context, currentUserContext)
    {
        ExpandLookupsMapping = LookupMappings.TaskExpandedMapping;
    }
}
