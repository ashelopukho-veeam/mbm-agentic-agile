using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mbt.webapi.BuiltIn;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Extensions;
using mbt.webapi.Services.Interfaces;
using mbt.webapi.UseCases.PaidMediaSet;
using MongoDB.Driver;

namespace mbt.webapi.Repositories;

public interface IPaidMediaSetRepository : IDbBaseRepository<PaidMediaSet>
{
    Task<PaidMediaSetWithLinkedItem> GetByIdExpanded(string id);
    Task<List<PaidMediaSetWithLinkedItem>> GetExpanded();
}

public class PaidMediaSetRepository : DbBaseRepository<PaidMediaSet>, IPaidMediaSetRepository
{
    public PaidMediaSetRepository(IDbContext context, ICurrentUserContext currentUserContext) : base(context,
        currentUserContext)
    {
    }

    public async Task<PaidMediaSetWithLinkedItem> GetByIdExpanded(string id)
    {
        var paidMediaSet = await GetAsync(id);

        if (paidMediaSet == null)
        {
            return null;
        }

        var linkedItemCollectionName = paidMediaSet.PaidMediaSetType switch
        {
            PaidMediaSetTypes.Delta => CollectionNames.Budgets,
            PaidMediaSetTypes.Transfer => CollectionNames.Transfers,
            PaidMediaSetTypes.IncrementalFund => CollectionNames.IncrementalFunds,
            _ => throw new ApiException("Invalid PaidMediaSetType")
        };

        var aggregate = BuildAggregate<PaidMediaSetWithLinkedItem>();
        aggregate = aggregate.Match(b => b.Id == id);

        aggregate = aggregate.ExpandLookups(LookupMappings.LookupAuditableMapping);
        aggregate = aggregate.ExpandLookups(
            LookupMappings.PaidMediaSetWithLinkedItemExpanded(linkedItemCollectionName));

        var result = await aggregate.FirstOrDefaultAsync();
        return result;
    }

    public async Task<List<PaidMediaSetWithLinkedItem>> GetExpanded()
    {
        var resultList = new List<PaidMediaSetWithLinkedItem>();

        var collectionByType = new List<Tuple<string, string>>()
        {
            new(PaidMediaSetTypes.Delta, CollectionNames.Budgets),
            new(PaidMediaSetTypes.Transfer, CollectionNames.Transfers),
            new(PaidMediaSetTypes.IncrementalFund, CollectionNames.IncrementalFunds)
        };

        foreach (var ct in collectionByType)
        {
            var aggregate = BuildAggregate<PaidMediaSetWithLinkedItem>().Match(p => p.PaidMediaSetType == ct.Item1);
            aggregate = aggregate.ExpandLookups(LookupMappings.LookupAuditableMapping);
            aggregate = aggregate.ExpandLookups(LookupMappings.PaidMediaSetWithLinkedItemExpanded(ct.Item2));

            var result = await aggregate.ToListAsync();
            if (result.Count > 0)
                resultList.AddRange(result);
        }

        return resultList;
    }
}
