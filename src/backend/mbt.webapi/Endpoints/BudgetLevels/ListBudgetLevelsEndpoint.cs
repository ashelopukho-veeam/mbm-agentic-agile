using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.BudgetLevels;

public class ListBudgetLevelsEndpoint : EndpointBaseAsync.WithoutRequest.WithResult<List<BudgetLevelDto>>
{
    private readonly IDbBaseRepository<BudgetLevel> _budgetLevelRepository;

    public ListBudgetLevelsEndpoint(IDbBaseRepository<BudgetLevel> budgetLevelRepository)
    {
        _budgetLevelRepository = budgetLevelRepository;
    }

    [HttpGet("api/levels")]
    [SwaggerOperation(
        Summary = "List budget levels",
        Description = "List budget levels",
        OperationId = "BudgetLevels.List",
        Tags = new[] { "BudgetLevels" })]
    public override async Task<List<BudgetLevelDto>> HandleAsync(
        CancellationToken cancellationToken = new())
    {
        var levels = await _budgetLevelRepository.GetAsync();
        var dtoLevelsList = levels.Select(BudgetLevelDto.FromBudgetLevel).ToList();

        return dtoLevelsList;
    }
}
