using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.Vendors;

public class List : EndpointBaseAsync.WithoutRequest.WithResult<List<VendorsDocument>>
{
    private readonly IDbBaseRepository<VendorsDocument> _vendorsRepository;

    public List(IDbBaseRepository<VendorsDocument> vendorsRepository)
    {
        _vendorsRepository = vendorsRepository;
    }

    [HttpGet("api/vendors")]
    [Authorize(Roles = AppRoles.ViewPolicy)]
    [SwaggerOperation(
        Summary = "List vendors",
        Description = "List vendors",
        OperationId = "Vendors.List",
        Tags = new[] { "Vendors" })]
    public override async Task<List<VendorsDocument>> HandleAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var items = await _vendorsRepository.GetAsync();
        return items;
    }
}
