using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;

namespace mbt.webapi.UseCases.Metadata;

public class RemoveMetadataTreeItemCommand : IRequest
{
    public string Id { get; set; }
}

[UsedImplicitly]
public class RemoveMetadataTreeItemCommandHandler : IRequestHandler<RemoveMetadataTreeItemCommand>
{
    private readonly IDbContext _context;

    public RemoveMetadataTreeItemCommandHandler(IDbContext context)
    {
        _context = context;
    }

    public async Task Handle(RemoveMetadataTreeItemCommand request, CancellationToken cancellationToken)
    {
        var matchStage = new BsonDocument
        {
            {
                "$match", new BsonDocument
                {
                    { "_id", request.Id }
                }
            }
        };

        var graphLookupStage = new BsonDocument
        {
            {
                "$graphLookup", new BsonDocument
                {
                    {
                        "from", CollectionNames.Metadata
                    },
                    { "startWith", "$_id" },
                    { "connectFromField", "_id" },
                    { "connectToField", "ParentId" },
                    { "as", "relatedItems" }
                }
            }
        };

        var pipeline = new[] { matchStage, graphLookupStage };

        var metadataCollection = _context.GetCollection<TreeItem>();

        var result = await metadataCollection
            .Aggregate<BsonDocument>(pipeline)
            .FirstAsync(cancellationToken);

        var relatedItemsToDelete = result["relatedItems"].AsBsonArray
            .Select(x => x.AsBsonDocument)
            .Select(x => x["_id"].AsString)
            .ToList();

        var itemsToDelete = relatedItemsToDelete.Append(request.Id).ToList();

        var filter = Builders<TreeItem>.Filter.In(x => x.Id, itemsToDelete);

        await metadataCollection.DeleteManyAsync(filter, cancellationToken);
    }
}
