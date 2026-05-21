using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using mbt.webapi.Domain.Entities.Common;
using mbt.webapi.Extensions;
using mbt.webapi.Services;
using mbt.webapi.Services.Interfaces;
using MongoDB.Driver;

namespace mbt.webapi.Repositories;

public interface IDbBaseRepositoryExpandable<T, TExpanded> : IDbBaseRepository<T> where TExpanded : IBaseIdItem
{
    public List<MongoLookup> ExpandLookupsMapping { get; init; }

    Task<PagedResult<TExpanded>> GetExpandedPaged(
        int pageStart = 0, int pageSize = 20, FilterDefinition<TExpanded> filter = null);

    Task<List<TExpanded>> GetExpanded(FilterDefinition<TExpanded> filter);

    Task<List<TExpanded>> GetExpanded(Expression<Func<TExpanded, bool>> filter);
    Task<List<TExpanded>> GetExpanded();

    Task<TExpanded> GetByIdExpanded(string id);
}

public class DbBaseRepositoryExpandable<T, TExpanded> : DbBaseRepository<T>, IDbBaseRepositoryExpandable<T, TExpanded>
    where
    T : IBaseIdItem
    where TExpanded : IBaseIdItem
{
    public List<MongoLookup> ExpandLookupsMapping { get; init; }

    public Task<PagedResult<TExpanded>> GetExpandedPaged(int pageStart = 0,
        int pageSize = 20, FilterDefinition<TExpanded> filter = null)
    {
        var aggregate = BuildAggregate<TExpanded>();
        if (filter != null)
        {
            aggregate = aggregate.Match(filter);
        }

        if (IsAuditable(typeof(T)))
        {
            aggregate = aggregate.ExpandLookups(LookupMappings.LookupAuditableMapping);
        }

        if (ExpandLookupsMapping is { Count: > 0 })
        {
            aggregate = aggregate.ExpandLookups(ExpandLookupsMapping);
        }

        return aggregate.ToPagedListAsync(pageStart, pageSize);
    }

    public Task<List<TExpanded>> GetExpanded(FilterDefinition<TExpanded> filter)
    {
        var aggregate = BuildAggregate<TExpanded>();
        if (filter != null)
        {
            aggregate = aggregate.Match(filter);
        }

        if (IsAuditable(typeof(T)))
        {
            aggregate = aggregate.ExpandLookups(LookupMappings.LookupAuditableMapping);
        }

        if (ExpandLookupsMapping is {Count: > 0})
        {
            aggregate = aggregate.ExpandLookups(ExpandLookupsMapping);
        }

        return aggregate.ToListAsync();
    }

    public Task<List<TExpanded>> GetExpanded(Expression<Func<TExpanded, bool>> filter)
    {
        var expressionFilter = new ExpressionFilterDefinition<TExpanded>(filter);
        return GetExpanded(expressionFilter);
    }

    public Task<List<TExpanded>> GetExpanded()
    {
        var aggregate = BuildAggregate<TExpanded>();

        if (IsAuditable(typeof(T)))
        {
            aggregate = aggregate.ExpandLookups(LookupMappings.LookupAuditableMapping);
        }

        if (ExpandLookupsMapping.Count > 0)
        {
            aggregate = aggregate.ExpandLookups(ExpandLookupsMapping);
        }

        return aggregate.ToListAsync();
    }

    public Task<TExpanded> GetByIdExpanded(string id)
    {
        var aggregate = BuildAggregate<TExpanded>();
        aggregate = aggregate.Match(b => b.Id == id);

        if (IsAuditable(typeof(T)))
        {
            aggregate = aggregate.ExpandLookups(LookupMappings.LookupAuditableMapping);
        }

        if (ExpandLookupsMapping.Count > 0)
        {
            aggregate = aggregate.ExpandLookups(ExpandLookupsMapping);
        }

        return aggregate.FirstOrDefaultAsync();
    }

    public DbBaseRepositoryExpandable(IDbContext context, ICurrentUserContext currentUserContext) : base(context,
        currentUserContext)
    {
    }
}
