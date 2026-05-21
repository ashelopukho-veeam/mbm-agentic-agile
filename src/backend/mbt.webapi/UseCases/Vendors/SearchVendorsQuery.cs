using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;

namespace mbt.webapi.UseCases.Vendors;

public class SearchVendorsQuery : IRequest<List<VendorsDocument>>
{
    public string Search { get; set; }

    public int Limit { get; set; } = 10;
}

public class SearchVendorsQueryHandler : IRequestHandler<SearchVendorsQuery, List<VendorsDocument>>
{
    public SearchVendorsQueryHandler(IDbContext dbContext)
    {
        Context = dbContext;
    }

    public IDbContext Context { get; }

    public async Task<List<VendorsDocument>> Handle(SearchVendorsQuery request, CancellationToken cancellationToken)
    {
        var queryExpr = new BsonRegularExpression(new Regex(Regex.Escape(request.Search), RegexOptions.IgnoreCase));
        var filter = new FilterDefinitionBuilder<VendorsDocument>().Regex(v => v.Title, queryExpr);

        var result = await Context.GetCollection<VendorsDocument>().Aggregate()
            .Match(filter)
            .SortBy(v => v.Title)
            .Limit(request.Limit)
            .ToListAsync(cancellationToken);

        return result;
    }
}
