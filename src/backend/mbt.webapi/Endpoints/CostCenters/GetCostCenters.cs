using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.CostCenters;

public class GetCostCenters : EndpointBaseAsync.WithoutRequest.WithActionResult<List<CostCenterDto>>
{
    private readonly IDbBaseRepository<CostCenter> _costCenterRepository;

    public GetCostCenters(IDbBaseRepository<CostCenter> costCenterRepository)
    {
        _costCenterRepository = costCenterRepository;
    }

    [HttpGet("api/costCenters")]
    [SwaggerOperation(
        Summary = "List cost centers",
        Description = "List cost centers",
        OperationId = "CostCenters.V2.List",
        Tags = new[] { "CostCenters" })]
    public override async Task<ActionResult<List<CostCenterDto>>> HandleAsync(
        CancellationToken cancellationToken = new())
    {
        var costCenters = await _costCenterRepository.GetAsync();
        var costCentersDtoList = costCenters.Select(c => new CostCenterDto(c.Id, c.Title, c.ShortTitle)).ToList();

        return costCentersDtoList;
    }
}
