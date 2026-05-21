using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.BudgetLevels;

public class
    GetByIdBudgetLevelEndpoint : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithActionResult<BudgetLevelDto>
{
    private readonly IDbBaseRepository<BudgetLevel> _budgetLevelRepository;

    public GetByIdBudgetLevelEndpoint(IDbBaseRepository<BudgetLevel> budgetLevelRepository)
    {
        _budgetLevelRepository = budgetLevelRepository;
    }

    [HttpGet("api/levels/{Id}")]
    [SwaggerOperation(
        Summary = "Get budget level by id",
        Description = "Get budget level by id",
        OperationId = "BudgetLevels.GetById",
        Tags = new[] { "BudgetLevels" })]
    public override async Task<ActionResult<BudgetLevelDto>> HandleAsync([FromRoute] ObjectIdRequest request,
        CancellationToken cancellationToken = new())
    {
        var budgetLevel = await _budgetLevelRepository.GetAsync(request.Id);

        if (budgetLevel == null)
            return NotFound();

        return BudgetLevelDto.FromBudgetLevel(budgetLevel);
    }
}
