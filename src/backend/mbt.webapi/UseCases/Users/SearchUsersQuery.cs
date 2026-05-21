using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;

namespace mbt.webapi.UseCases.Users;

public class SearchUsersQuery : IRequest<List<UserProfile>>
{
    public string Search { get; set; }

    public int Limit { get; set; } = 10;
}

public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, List<UserProfile>>
{
    public SearchUsersQueryHandler(IDbContext dbContext)
    {
        Context = dbContext;
    }

    public IDbContext Context { get; }

    public async Task<List<UserProfile>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        var queryExpr = new BsonRegularExpression(new Regex(request.Search, RegexOptions.IgnoreCase));
        var builder = Builders<UserProfile>.Filter;
        var filter = builder.Regex("DisplayName", queryExpr);

        var result = await Context.GetCollection<UserProfile>()
            .Aggregate()
            .Match(filter)
            .Limit(request.Limit)
            .ToListAsync(cancellationToken);

        return result;
    }
}
