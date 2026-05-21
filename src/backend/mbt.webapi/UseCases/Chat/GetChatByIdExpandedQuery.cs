using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;

namespace mbt.webapi.UseCases.Chat;

public class GetChatByIdExpandedRequest : IRequest<List<ChatMessageExpanded>>
{
    public string Id { get; set; }
}

public class GetChatByIdExpandedRequestHandler : IRequestHandler<GetChatByIdExpandedRequest, List<ChatMessageExpanded>>
{
    private readonly IDbContext _dbContext;

    public GetChatByIdExpandedRequestHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ChatMessageExpanded>> Handle(GetChatByIdExpandedRequest request,
        CancellationToken cancellationToken)
    {
        var pipeline = new[]
        {
            new BsonDocument("$match",
                new BsonDocument("ParentId", request.Id)),
            new BsonDocument("$unwind",
                new BsonDocument("path", "$Messages")),
            new BsonDocument("$replaceRoot",
                new BsonDocument("newRoot", "$Messages")),
            new BsonDocument("$lookup",
                new BsonDocument
                {
                    { "from", "users" },
                    { "localField", "CreatedBy" },
                    { "foreignField", "_id" },
                    { "as", "CreatedByUser" }
                }),
            new BsonDocument("$unwind",
                new BsonDocument
                {
                    { "path", "$CreatedByUser" },
                    { "preserveNullAndEmptyArrays", true }
                })
        };

        var chatCollection = _dbContext.GetCollection<Domain.Entities.Chat>();

        var p = await chatCollection.AggregateAsync<ChatMessageExpanded>(pipeline,
            cancellationToken: cancellationToken);

        var result = p.ToList();

        return result;
    }
}
