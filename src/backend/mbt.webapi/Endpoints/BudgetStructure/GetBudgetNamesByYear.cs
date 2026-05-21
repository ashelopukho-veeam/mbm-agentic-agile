using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.BuiltIn;
using mbt.webapi.Endpoints.BudgetStructure.dto;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.BudgetStructure;

public class GetBudgetNamesByYear : EndpointBaseAsync.WithRequest<int>.WithActionResult<List<BudgetNamesByYearDto>>
{
    private readonly IBudgetService _budgetService;

    public GetBudgetNamesByYear(IBudgetService budgetService)
    {
        _budgetService = budgetService;
    }

    [HttpGet("api/budgetStructure/names/{year}")]
    [Authorize(Roles = AppRoles.ViewPolicy)]
    [SwaggerOperation(
        Summary = "List budget structures short info by year",
        Description = "List budget structures short info by year",
        OperationId = "BudgetStructure.GetBudgetNamesByYear",
        Tags = new[] { "BudgetStructure" })]
    public override async Task<ActionResult<List<BudgetNamesByYearDto>>> HandleAsync([FromRoute] int year,
        CancellationToken cancellationToken = new())
    {
        var budgetNamesByYears = await _budgetService.GetByYear(year);

        var resultDtoCollection = budgetNamesByYears.Select(BudgetNamesByYearDto.FromBudgetNamesByYear).ToList();

        return resultDtoCollection;
    }
}
