using System.Linq;
using System.Threading.Tasks;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace mbt.webapi.Repositories;

public class MetadataRepository : DbBaseRepository<TreeItem>
{
    public MetadataRepository(IDbContext dbcontext, ICurrentUserContext userContext) : base(dbcontext, userContext)
    {
        Collection = dbcontext.GetCollection<TreeItem>();
    }

    public override void Remove(string id)
    {
        var itemsToDeletePipelineDefinition = ItemsToDeletePipeline(id);

        var itemsToRemove = Collection.Aggregate(itemsToDeletePipelineDefinition).ToList();

        var ids = itemsToRemove.Select(l => l["_id"].ToString()).ToArray();
        var deleteItemsFilter = Builders<TreeItem>.Filter.In("_id", ids);

        Collection.DeleteMany(deleteItemsFilter);
    }

    public override async Task RemoveAsync(string id)
    {
        var itemsToDeletePipelineDefinition = ItemsToDeletePipeline(id);

        var itemsToRemove = await Collection.Aggregate(itemsToDeletePipelineDefinition).ToListAsync();

        var ids = itemsToRemove.Select(l => l["_id"].ToString()).ToArray();
        var deleteItemsFilter = Builders<TreeItem>.Filter.In("_id", ids);

        await Collection.DeleteManyAsync(deleteItemsFilter);
    }

    private PipelineDefinition<TreeItem, BsonDocument> ItemsToDeletePipeline(string rootId)
    {
        PipelineDefinition<TreeItem, BsonDocument> pipelineDefinition = new[]
        {
            new BsonDocument("$graphLookup",
                new BsonDocument
                {
                    { "from", "metadata" },
                    { "startWith", "$ParentId" },
                    { "connectFromField", "ParentId" },
                    { "connectToField", "_id" },
                    { "as", "idds" }
                }),
            new BsonDocument("$match",
                new BsonDocument("$or",
                    new BsonArray
                    {
                        new BsonDocument("idds._id", rootId),
                        new BsonDocument("_id", rootId)
                    }))
        };

        return pipelineDefinition;
    }
}
