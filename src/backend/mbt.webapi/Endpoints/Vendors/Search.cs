using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using mbt.webapi.UseCases.Vendors;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using VendorsDocument = mbt.webapi.Domain.Entities.VendorsDocument;

namespace mbt.webapi.Endpoints.Vendors;

public class Search : EndpointBaseAsync.WithRequest<SearchVendorsRequest>.WithActionResult<List<VendorsDocument>>
{
    private readonly IMediator _mediator;
    private readonly IValidator<SearchVendorsRequest> _validator;

    public Search(IMediator mediator, IValidator<SearchVendorsRequest> validator)
    {
        this._mediator = mediator;
        _validator = validator;
    }

    [HttpPost(SearchVendorsRequest.Route)]
    [SwaggerOperation(
        Summary = "Search vendors",
        Description = "Search vendors",
        OperationId = "Search.Vendors",
        Tags = new[] { "Vendors" })]
    public override async Task<ActionResult<List<VendorsDocument>>> HandleAsync(
        [FromBody] SearchVendorsRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var query = new SearchVendorsQuery()
        {
            Search = request.Search,
            Limit = request.Limit
        };

        var result = await _mediator.Send(query, cancellationToken);

        return result;
    }
}
