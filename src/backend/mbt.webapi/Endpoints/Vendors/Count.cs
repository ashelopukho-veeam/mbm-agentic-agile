using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.Vendors;

public class Count : EndpointBaseAsync.WithoutRequest.WithActionResult<long>
{
    private readonly IDbBaseRepository<VendorsDocument> _vendorsRepository;

    public Count(IDbBaseRepository<VendorsDocument> vendorsRepository)
    {
        _vendorsRepository = vendorsRepository;
    }

    [HttpGet("api/vendors/count")]
    [SwaggerOperation(
        Summary = "Count vendors",
        Description = "Count vendors",
        OperationId = "Count.Vendors",
        Tags = new[] { "Vendors" })]
    public override async Task<ActionResult<long>> HandleAsync(
        CancellationToken cancellationToken = new())
    {
        var result = await _vendorsRepository.CountAsync();

        return result;
    }
}
