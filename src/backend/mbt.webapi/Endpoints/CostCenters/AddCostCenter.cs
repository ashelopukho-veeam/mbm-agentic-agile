using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.BuiltIn;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.CostCenters;

public record AddCostCenterRequest(string Title, string ShortTitle);

public class AddCostCenter : EndpointBaseAsync.WithRequest<AddCostCenterRequest>.WithActionResult<CostCenterDto>
{
    private readonly IDbBaseRepository<CostCenter> _costCenterRepository;

    public AddCostCenter(IDbBaseRepository<CostCenter> costCenterRepository)
    {
        _costCenterRepository = costCenterRepository;
    }

    [HttpPost("api/costCenters")]
    [Authorize(Roles = AppRoles.AdminPolicy)]
    [SwaggerOperation(
        Summary = "Add cost center",
        Description = "Add cost center",
        OperationId = "CostCenters.V2.Add",
        Tags = new[] { "CostCenters" })]
    public override async Task<ActionResult<CostCenterDto>> HandleAsync(
        [FromBody] AddCostCenterRequest request, CancellationToken cancellationToken = new())
    {
        var withSameShortTitle = await _costCenterRepository.FindOneAsync(c => c.ShortTitle == request.ShortTitle);
        if (withSameShortTitle != null)
        {
            throw new ApiException("Cost center with the same short title already exists");
        }

        var c = new CostCenter
        {
            ShortTitle = request.ShortTitle,
            Title = request.Title
        };

        await _costCenterRepository.CreateAsync(c);

        var resultDto = new CostCenterDto(c.Id, c.Title, c.ShortTitle);

        return resultDto;
    }
}
