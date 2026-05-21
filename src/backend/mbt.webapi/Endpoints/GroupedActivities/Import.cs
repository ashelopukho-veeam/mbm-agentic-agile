using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.GroupedActivities;

public class Import : EndpointBaseAsync.WithRequest<ImportGroupedActivitiesRequest>.WithoutResult
{
    private readonly IGroupActivitiesService _groupActivitiesService;
    private readonly IBudgetService _budgetService;
    private readonly IValidator<ImportGroupedActivitiesRequest> _validator;

    public Import(IGroupActivitiesService groupActivitiesService, IBudgetService budgetService,
        IValidator<ImportGroupedActivitiesRequest> validator)
    {
        _groupActivitiesService = groupActivitiesService;
        _budgetService = budgetService;
        _validator = validator;
    }

    [Authorize(Roles = AppRoles.ManageGroupedActivities)]
    [HttpPost("api/groupedActivities/import")]
    [SwaggerOperation(
        Summary = "Import grouped activities",
        Description = "Import grouped activities",
        OperationId = "GroupedActivities.Import",
        Tags = new[] { "GroupedActivities" })]
    public override async Task<ActionResult> HandleAsync([FromForm] ImportGroupedActivitiesRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var budget = await _budgetService.GetBudgetByPlanId(request.BudgetPlanId);

        if (budget == null) throw new ApiException($"Budget Plan not found. Id: {request.BudgetPlanId}");

        if (!budget.IsPaidMedia && !User.IsInRole(AppRoles.Admins) && !User.IsInRole(AppRoles.SysAdmins))
            throw new ApiException("Bulk upload available for Paid Media budgets only.");

        await _groupActivitiesService.ImportFromCsv(budget, request.BudgetPlanId, request.FormFile);

        return NoContent();
    }
}
