using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Services.Interfaces;
using MongoDB.Driver;

namespace mbt.webapi.Repositories;

public interface IBudgetRepository : IDbBaseRepositoryExpandable<Budget, BudgetStructureExpanded>
{
    Task<List<int>> GetDistinctYears(CancellationToken cancellationToken);
}

public class BudgetRepository : DbBaseRepositoryExpandable<Budget, BudgetStructureExpanded>, IBudgetRepository
{
    public BudgetRepository(IDbContext context, ICurrentUserContext currentUserContext) : base(context,
        currentUserContext)
    {
        ExpandLookupsMapping = LookupMappings.BudgetStructureUsersExpanded;
    }

    public async Task<List<int>> GetDistinctYears(CancellationToken cancellationToken)
    {
        var filter = Builders<Budget>.Filter.Ne(b => b.IsDeleted, true);
        var yearFieldDefinition = new ExpressionFieldDefinition<Budget, int>(x => x.Year);

        var result = await Collection.DistinctAsync(yearFieldDefinition, filter, cancellationToken: cancellationToken);

        return result.ToList(cancellationToken: cancellationToken).OrderBy(year => year).ToList();
    }
}
