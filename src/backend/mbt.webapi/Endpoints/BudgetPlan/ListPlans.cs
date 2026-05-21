using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Extensions;
using mbt.webapi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.BudgetPlan;

public class ListPlans : EndpointBaseAsync.WithRequest<ListBudgetPlansRequest>.WithActionResult<
    List<BudgetPlanListItemDto>>
{
    private readonly IBudgetRepository _budgetStructureRepository;

    public ListPlans(IBudgetRepository budgetStructureRepository)
    {
        _budgetStructureRepository = budgetStructureRepository;
    }

    [HttpGet("api/budgetPlans")]
    [Authorize(Roles = AppRoles.ViewPolicy)]
    [SwaggerOperation(
        Summary = "List Budget Plans",
        Description = "List Budget Plans",
        OperationId = "BudgetPlans.ListPlans",
        Tags = new[] { "BudgetPlans" })]
    public override async Task<ActionResult<List<BudgetPlanListItemDto>>> HandleAsync(
        [FromQuery] ListBudgetPlansRequest request, CancellationToken cancellationToken = new CancellationToken())
    {
        var budgets = await _budgetStructureRepository.GetExpanded(
            b => b.Year == request.Year &&
                 (b.Status == BudgetStatus.Approved || b.Status == BudgetStatus.Inactive));

        var result = new List<BudgetPlanListItemDto>();

        foreach (var budget in budgets)
        {
            var plan = budget.Plans.GetActivePlanByQuarter(request.Plan);
            if (plan != null)
                result.Add(CreateDto(budget, plan));
        }

        return result;
    }


    private BudgetPlanListItemDto CreateDto(BudgetStructureExpanded budget, mbt.webapi.Domain.Entities.BudgetPlan plan)
    {
        return new BudgetPlanListItemDto
        {
            Id = plan.Id,
            BudgetId = budget.Id,
            Title = budget.Title,
            Status = plan.Status,
            BudgetType = budget.BudgetType,
            Level1 = budget.Level1,
            Level2 = budget.Level2,
            Level3 = budget.Level3,
            Created = budget.Created,
            Modified = budget.Modified,
            CostCenter = budget.CostCenter,
            Owner = budget.Owner,
            ParentManager = budget.ParentManager,
            Manager = budget.Manager,
            CreatedByUser = budget.CreatedByUser,
            ModifiedByUser = budget.ModifiedByUser,
            Q1 = plan.Q1,
            Q2 = plan.Q2,
            Q3 = plan.Q3,
            Q4 = plan.Q4,
            Total = plan.Q1 + plan.Q2 + plan.Q3 + plan.Q4,
            Quarter = plan.Quarter
        };
    }
}

[PublicAPI]
public record ListBudgetPlansRequest
{
    public string Plan { get; set; }
    public int Year { get; set; }
}

[PublicAPI]
public class BudgetPlanListItemDto
{
    public string Id { get; set; }
    public string BudgetId { get; set; }
    public string Title { get; set; }
    public string Quarter { get; set; }
    public string Status { get; set; }
    public string BudgetType { get; set; }
    public string Level1 { get; set; }
    public string Level2 { get; set; }
    public string Level3 { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public string CostCenter { get; set; }
    public UserProfile Owner { get; set; }
    public UserProfile ParentManager { get; set; }
    public UserProfile Manager { get; set; }
    public UserProfile CreatedByUser { get; set; }
    public UserProfile ModifiedByUser { get; set; }
    public double Q1 { get; set; }
    public double Q2 { get; set; }
    public double Q3 { get; set; }
    public double Q4 { get; set; }
    public double Total { get; set; }
}
