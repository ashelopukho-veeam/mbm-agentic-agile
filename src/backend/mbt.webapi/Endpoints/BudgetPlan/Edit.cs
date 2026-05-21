using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Endpoints.BudgetPlan.dto;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.BudgetPlan;

public class Edit : EndpointBaseAsync.WithRequest<EditBudgetPlanRequest>.WithActionResult
{
    private readonly IBudgetService _budgetService;
    private readonly IApiService _apiService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IValidator<EditBudgetPlanRequest> _validator;
    private readonly IDbBaseRepository<BudgetPlanHistoryItem> _budgetPlanHistoryRepository;
    private readonly IMapper _mapper;


    public Edit(IBudgetService budgetService, IApiService apiService, ICurrentUserContext currentUserContext,
        IValidator<EditBudgetPlanRequest> validator, IDbBaseRepository<BudgetPlanHistoryItem> budgetPlanHistoryRepository, IMapper mapper)
    {
        _budgetService = budgetService;
        _apiService = apiService;
        _currentUserContext = currentUserContext;
        _validator = validator;
        _budgetPlanHistoryRepository = budgetPlanHistoryRepository;
        _mapper = mapper;
    }

    [HttpPut(BudgetPlanRoutes.Update)]
    [Authorize(Roles = $"{AppRoles.AdminPolicy},{AppRoles.Contributors}")]
    [SwaggerOperation(
        Summary = "Edit a budget plan",
        Description = "Edit a budget plans",
        OperationId = "BudgetPlans.Edit",
        Tags = new[] { "BudgetPlans" })]
    public override async Task<ActionResult> HandleAsync(EditBudgetPlanRequest request,
        CancellationToken cancellationToken = new())
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var config = await _apiService.GetCommonConfigAsync();
        if (!config.AllowCreateReforecasts)
            throw new ApiException(ErrorMessages.ReforecastsAreNotAllowed);


        var budget = await _budgetService.GetBudgetByPlanId(request.BudgetPlanId) ?? throw new ApiException(ErrorMessages.NotFound);

        ValidatePermissions(budget);

        var plan = budget.GetBudgetPlanById(request.BudgetPlanId);
        if (plan.Status != BudgetPlanStatus.Draft)
        {
            throw new ApiException(ErrorMessages.BudgetIsNotInDraftStatus);
        }

        plan.Q1 = request.Q1;
        plan.Q2 = request.Q2;
        plan.Q3 = request.Q3;
        plan.Q4 = request.Q4;
        plan.Segments = request.Segments;
        plan.Campaigns = request.Campaigns;
        await _budgetService.UpdateAsync(budget);

        var historyItem = _mapper.Map<BudgetPlanHistoryItem>(plan);
        await _budgetPlanHistoryRepository.CreateAsync(historyItem);

        return Ok();
    }

    private void ValidatePermissions(Budget budget)
    {
        var isAdmin = _currentUserContext.IsInRoles(new[] { AppRoles.SysAdmins, AppRoles.Admins });

        var currentUserId = _currentUserContext.UserId;

        if (!isAdmin &&
            currentUserId != budget.ParentManagerId &&
            currentUserId != budget.OwnerId &&
            currentUserId != budget.ManagerId)
            throw new AccessDeniedException();
    }
}
