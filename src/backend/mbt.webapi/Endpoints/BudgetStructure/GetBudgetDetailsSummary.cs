using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.BudgetStructure;

public class
    GetBudgetDetailsSummary : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithActionResult<BudgetDetailsSummaryDto>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly ITransfersService _transfersService;
    private readonly IIncrementalFundsService _incrementalFundsService;
    private readonly IGroupActivitiesService _groupActivitiesService;
    private readonly IValidator<ObjectIdRequest> _validator;

    public GetBudgetDetailsSummary(ITransfersService transfersService,
        IGroupActivitiesService groupActivitiesService, IIncrementalFundsService incrementalFundsService,
        IValidator<ObjectIdRequest> validator, IBudgetRepository budgetRepository)
    {
        _transfersService = transfersService;
        _groupActivitiesService = groupActivitiesService;
        _incrementalFundsService = incrementalFundsService;
        _validator = validator;
        _budgetRepository = budgetRepository;
    }

    [HttpGet("api/budgetStructure/getBudgetDetailsSummary/{Id}")]
    [Authorize(Roles = AppRoles.ViewPolicy)]
    [SwaggerOperation(
        Summary = "Get budget details summary",
        Description = "Get budget details summary",
        OperationId = "BudgetStructure.GetByBudgetId",
        Tags = new[] { "BudgetStructure" })]
    public override async Task<ActionResult<BudgetDetailsSummaryDto>> HandleAsync([FromRoute] ObjectIdRequest request,
        CancellationToken cancellationToken = new CancellationToken())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var budget = await _budgetRepository.GetAsync(request.Id);

        if (budget == null)
            return NotFound();

        var finalPlan = budget.Plans.LastOrDefault(p => p.Status == BudgetPlanStatus.Final);

        var budgetTransfers = await _transfersService.GetTransfersForBudget(request.Id);
        var budgetIncrementalFunds = await _incrementalFundsService.GetByBudgetIdAsync(request.Id);

        var finalPlanTransfers = finalPlan != null
            ? budgetTransfers.Where(t => t.Plan == finalPlan.Quarter)
                .Where(t => t.Status is TransferStatus.Approved or TransferStatus.PendingApproval)
                .ToList()
            : new List<Transfer>();

        var finalPlanIncrementalFunds = finalPlan != null
            ? budgetIncrementalFunds.Where(f => f.Plan == finalPlan.Quarter)
                .Where(f => f.Status is IncrementalFundStatus.Approved or IncrementalFundStatus.PendingApproval)
                .ToList()
            : new List<IncrementalFund>();

        // current active draft plan for the budget
        var currentDraftPlan = budget.Plans.FirstOrDefault(p => p.Status == BudgetPlanStatus.Draft);

        var budgetGroupedActivities = currentDraftPlan != null
            ? await _groupActivitiesService.GetByBudgetPlanId(currentDraftPlan.Id)
            : new List<GroupedActivity>();


        var result = new BudgetDetailsSummaryDto()
        {
            Transfers = CalculateTransfersQuarterValues(budget.Id, finalPlanTransfers),
            IncrementalFunds = CalculateIncrementalFundsQuarterValues(finalPlanIncrementalFunds),
            GroupedActivities = CalculateGroupedActivitiesQuarterValues(budgetGroupedActivities)
        };

        return result;
    }

    private QuarterValues CalculateGroupedActivitiesQuarterValues(List<GroupedActivity> budgetGroupedActivities)
    {
        var quarterValues = new QuarterValues();
        foreach (var activity in budgetGroupedActivities)
        {
            quarterValues.SetQuarterValue("Q" + activity.Quarter, activity.NetPlannedAmount);
        }

        return quarterValues;
    }


    private QuarterValues CalculateTransfersQuarterValues(string budgetId, List<Transfer> budgetTransfers)
    {
        var quarterValues = new QuarterValues();
        foreach (var transfer in budgetTransfers)
        {
            var isFrom = transfer.FromBudgetId == budgetId;
            var q = isFrom ? transfer.FromQuarter : transfer.ToQuarter;
            quarterValues.SetQuarterValue(q, isFrom ? -transfer.Amount : transfer.Amount);
        }

        return quarterValues;
    }

    private QuarterValues CalculateIncrementalFundsQuarterValues(List<IncrementalFund> budgetIncrementalFunds)
    {
        var quarterValues = new QuarterValues();
        foreach (var fund in budgetIncrementalFunds)
        {
            quarterValues.SetQuarterValue("Q" + fund.ToQuarter, fund.Amount);
        }

        return quarterValues;
    }
}

public class QuarterValues
{
    public double Q1 { get; set; }
    public double Q2 { get; set; }
    public double Q3 { get; set; }
    public double Q4 { get; set; }

    public void SetQuarterValue(string quarter, double value)
    {
        if (quarter == "Q1") Q1 += value;
        if (quarter == "Q2") Q2 += value;
        if (quarter == "Q3") Q3 += value;
        if (quarter == "Q4") Q4 += value;
    }
}

public class BudgetDetailsSummaryDto
{
    public QuarterValues Transfers { get; set; }
    public QuarterValues IncrementalFunds { get; set; }
    public QuarterValues GroupedActivities { get; set; }
}
