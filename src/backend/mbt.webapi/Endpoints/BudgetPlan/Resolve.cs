using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Services;
using mbt.webapi.Services.Interfaces;
using mbt.webapi.Utils;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.BudgetPlan;

public class ResolveBudgetPlan : EndpointBaseAsync.WithRequest<ResolveBudgetPlanData>.WithResult<BulkOperationResult>
{
    private readonly IBudgetPlanService _budgetPlanService;
    private readonly IBudgetService _budgetService;

    public ResolveBudgetPlan(IBudgetPlanService budgetPlanService, IBudgetService budgetService)
    {
        _budgetPlanService = budgetPlanService;
        _budgetService = budgetService;
    }

    [HttpPost("api/budgetPlan/resolve")]
    [SwaggerOperation(
        Summary = "Resolve budget plan",
        Description = "Resolve budget plan",
        OperationId = "BudgetPlans.ResolveBudgetPlan",
        Tags = new[] { "BudgetPlans" })]
    public override async Task<BulkOperationResult> HandleAsync(ResolveBudgetPlanData request,
        CancellationToken cancellationToken = new())
    {
        var result = await BulkOperation.Run(request.BudgetPlanIds,
            async budgetPlanId => { await Resolve(budgetPlanId, request.Outcome, request.Comment, request.Notify); },
            "Resolve budget plans");

        return result;
    }

    private async Task ValidateBudgetAndPlan(string budgetPlanId)
    {
        var budget = await _budgetService.GetBudgetByPlanId(budgetPlanId);

        if (budget == null)
            throw new ApiException("Budget plan not found");

        var budgetPlan = budget.GetBudgetPlanById(budgetPlanId);

        if (budgetPlan == null)
            throw new ApiException("Budget plan not found");

        // validate  budget plan campaign& segments values
        var segmentValues = budgetPlan.Segments.Sum(s => s.Value);
        var campaignValues = budgetPlan.Campaigns.Sum(c => c.Value);
        if (Convert.ToInt32(segmentValues) != 100)
            throw new ApiException("Segment values must sum to 100");
        if (Convert.ToInt32(campaignValues) != 100)
            throw new ApiException("Campaign values must sum to 100");
    }

    private async Task<Func<string, string, bool, Task>> GetResolveStepFunc(string budgetPlanId, string outcome)
    {
        var budget = await _budgetService.GetBudgetByPlanId(budgetPlanId);
        var budgetPlan = budget.GetBudgetPlanById(budgetPlanId);
        var isOriginalForecast = budgetPlan.Quarter == Quarters.Q1;

        return budgetPlan.Status switch
        {
            // original plan
            BudgetPlanStatus.Draft when outcome == MbtTaskResolveAction.Approve => _budgetPlanService
                .DraftToSubmitToOwnerStep,
            BudgetPlanStatus.SubmittedToOwner when isOriginalForecast && outcome == MbtTaskResolveAction.Reject =>
                _budgetPlanService.SubmittedToOwnerToDraftInOriginalForecastStep,
            BudgetPlanStatus.SubmittedToOwner when isOriginalForecast && outcome == MbtTaskResolveAction.Approve =>
                _budgetPlanService.SubmittedToOwnerToPendingApprovalInOriginalForecastStep,
            BudgetPlanStatus.PendingApproval when isOriginalForecast && outcome == MbtTaskResolveAction.Reject =>
                _budgetPlanService.PendingApprovalToDraftInOriginalForecastStep,
            BudgetPlanStatus.PendingApproval when isOriginalForecast && outcome == MbtTaskResolveAction.Approve =>
                _budgetPlanService.PendingApprovalToApprovedInOriginalForecastStep,
            // reforecast
            BudgetPlanStatus.SubmittedToOwner when !isOriginalForecast && outcome == MbtTaskResolveAction.Reject =>
                _budgetPlanService
                    .SubmittedToOwnerToDraftInReforecastStep,
            BudgetPlanStatus.SubmittedToOwner when !isOriginalForecast && outcome == MbtTaskResolveAction.Approve =>
                _budgetPlanService
                    .SubmittedToOwnerToApprovedInReforecastStep,
            _ => throw new ApiException(ErrorMessages.InvalidBudgetPlanStatus),
        };
    }

    private async Task Resolve(string budgetPlanId, string outcome, string comment, bool? notify)
    {
        await ValidateBudgetAndPlan(budgetPlanId);
        var stepFunc = await GetResolveStepFunc(budgetPlanId, outcome);
        await stepFunc(budgetPlanId, comment, notify.GetValueOrDefault(false));
    }
}

[PublicAPI]
public class ResolveBudgetPlanData
{
    public string Outcome { get; set; }
    public string Comment { get; set; }
    public List<string> BudgetPlanIds { get; set; }

    public bool? Notify { get; set; }
}

public class ResolveBudgetPlanDataValidator : AbstractValidator<ResolveBudgetPlanData>
{
    public ResolveBudgetPlanDataValidator()
    {
        RuleFor(x => x.Outcome)
            .Must(x => x is MbtTaskResolveAction.Approve or MbtTaskResolveAction.Reject)
            .WithMessage("Invalid outcome. Only 'Approve' and 'Reject' are supported");
        RuleForEach(v => v.BudgetPlanIds)
            .NotEmpty()
            .Length(BuiltInConstants.ObjectIdLength);

        RuleFor(x => x.BudgetPlanIds).NotEmpty();
    }
}
