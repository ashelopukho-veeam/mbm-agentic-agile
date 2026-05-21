using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.GroupedActivities;

public class GetBudgetSummaryReport : EndpointBaseAsync.WithRequest<GetBudgetSummaryReportRequest>.WithActionResult<
    BudgetGroupedActivitySummary>
{
    private readonly IGroupActivitiesService _groupActivitiesService;
    private readonly IBudgetRepository _budgetRepository;

    public GetBudgetSummaryReport(IGroupActivitiesService groupActivitiesService,  IBudgetRepository budgetRepository)
    {
        _groupActivitiesService = groupActivitiesService;
        _budgetRepository = budgetRepository;
    }

    [HttpGet("api/groupedActivities/getBudgetSummaryReport/{BudgetId}")]
    [SwaggerOperation(
        Summary = "Get a grouped activities summary report by Budget Id",
        Description = "Get a grouped activities summary report by Budget Id",
        OperationId = "GroupedActivities.getBudgetSummaryReport",
        Tags = new[] { "GroupedActivities" })]
    public override async Task<ActionResult<BudgetGroupedActivitySummary>> HandleAsync(
        [FromRoute] GetBudgetSummaryReportRequest request,
        CancellationToken cancellationToken = new())
    {
        var budget = await _budgetRepository.GetAsync(request.BudgetId);

        if (budget == null)
            return NotFound();

        var plans =
            budget.Plans.Select(async p => await _groupActivitiesService.GetSummaryReport(p.Id))
                .Select(t => t.Result).ToList();

        BudgetGroupedActivitySummary report = new()
        {
            BudgetId = budget.Id,
            Plans = plans
        };

        return report;
    }
}
