using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.BuiltIn;
using mbt.webapi.Endpoints.BudgetPlan.dto;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using BudgetPlanConfig = mbt.webapi.Domain.Entities.BudgetPlanConfig;

namespace mbt.webapi.Endpoints.BudgetPlan;

public class GetBudgetPlanConfig : EndpointBaseAsync.WithoutRequest.WithActionResult<BudgetPlanConfigDto>
{
    private readonly IApiService _apiService;

    public GetBudgetPlanConfig(IApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet("api/budgetPlan/config")]
    [Authorize(Roles = AppRoles.ViewPolicy)]
    [SwaggerOperation(
        Summary = "Get budget plans config",
        Description = "Get budget plans config",
        OperationId = "BudgetPlans.Config",
        Tags = new[] { "BudgetPlans" })]
    public override async Task<ActionResult<BudgetPlanConfigDto>> HandleAsync(
        CancellationToken cancellationToken = new())
    {
        var budgetPlanConfig = await _apiService.GetBudgetPlanConfig() ?? new BudgetPlanConfig();

        return BudgetPlanConfigDto.FromBudgetPlanConfig(budgetPlanConfig);
    }
}
