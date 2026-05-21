using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mbt.webapi.Domain.Entities.Common;
using mbt.webapi.Repositories;
using MongoDB.Driver;

namespace mbt.webapi.Extensions;

public static class MGenericExt
{
    public static IAggregateFluent<TResult> ExpandLookups<TResult>(this IAggregateFluent<TResult> aggregate,
        IEnumerable<MongoLookup> lookupMappingList)
    {
        var opt = new AggregateUnwindOptions<TResult>
        {
            PreserveNullAndEmptyArrays = true
        };

        var result = lookupMappingList.Aggregate(aggregate,
            (current, lookupMappingItem) => current
                .Lookup(lookupMappingItem.CollectionName, lookupMappingItem.SourceField,
                    lookupMappingItem.TargetField, lookupMappingItem.Alias)
                .Unwind(lookupMappingItem.Alias, opt));

        return result;
    }

    public static IAggregateFluent<T> ExcludeDeleted<T>(this IAggregateFluent<T> aggregate) where T : IIsDeleted
    {
        return aggregate.Match(b => b.IsDeleted != true);
    }
}

public static class MongoCollectionQueryByPageExtensions
{
    public static async Task<(int totalPages, IReadOnlyList<TDocument> data)> AggregateByPage<TDocument>(
        this IMongoCollection<TDocument> collection,
        FilterDefinition<TDocument> filterDefinition,
        SortDefinition<TDocument> sortDefinition,
        int page,
        int pageSize)
    {
        var countFacet = AggregateFacet.Create("count",
            PipelineDefinition<TDocument, AggregateCountResult>.Create(new[]
            {
                PipelineStageDefinitionBuilder.Count<TDocument>()
            }));

        var dataFacet = AggregateFacet.Create("data",
            PipelineDefinition<TDocument, TDocument>.Create(new[]
            {
                PipelineStageDefinitionBuilder.Sort(sortDefinition),
                PipelineStageDefinitionBuilder.Skip<TDocument>((page - 1) * pageSize),
                PipelineStageDefinitionBuilder.Limit<TDocument>(pageSize)
            }));


        var aggregation = await collection.Aggregate()
            .Match(filterDefinition)
            .Facet(countFacet, dataFacet)
            .ToListAsync();

        var count = aggregation.First()
            .Facets.First(x => x.Name == "count")
            .Output<AggregateCountResult>()
            ?.FirstOrDefault()
            ?.Count;

        var totalPages = (int)Math.Ceiling((double)count / pageSize);

        var data = aggregation.First()
            .Facets.First(x => x.Name == "data")
            .Output<TDocument>();

        return (totalPages, data);
    }
}
