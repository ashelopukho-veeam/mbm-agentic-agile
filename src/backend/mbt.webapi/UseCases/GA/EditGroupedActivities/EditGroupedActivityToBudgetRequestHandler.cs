using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using MediatR;

namespace mbt.webapi.UseCases.GA.EditGroupedActivities;

public class
    EditGroupedActivityToBudgetRequestHandler : IRequestHandler<EditGroupedActivityToBudgetRequest, GroupedActivity>
{
    private readonly IApiService _apiService;
    private readonly IDbBaseRepository<Budget> _budgetRepository;
    private readonly ICurrencyService _currencyService;
    private readonly IDbBaseRepository<GroupedActivity> _gaRepository;
    private readonly IValidator<IWithPaidMediaData> _paidMediaFieldsValidator;

    public EditGroupedActivityToBudgetRequestHandler(IDbBaseRepository<Budget> budgetRepository,
        IDbBaseRepository<GroupedActivity> gaRepository, IApiService apiService, ICurrencyService currencyService,
        IValidator<IWithPaidMediaData> paidMediaFieldsValidator)
    {
        _budgetRepository = budgetRepository;
        _gaRepository = gaRepository;
        _apiService = apiService;
        _currencyService = currencyService;
        _paidMediaFieldsValidator = paidMediaFieldsValidator;
    }

    public async Task<GroupedActivity> Handle(EditGroupedActivityToBudgetRequest request,
        CancellationToken cancellationToken)
    {
        var ga = await _gaRepository.GetAsync(request.Id);
        if (ga == null)
            throw new ApiException(ErrorMessages.GroupedActivityNotFound);


        // check if Ga is allowed to be edited
        var budget = await _budgetRepository.GetAsync(ga.BudgetId);

        if (budget.IsPaidMedia)
        {
            await _paidMediaFieldsValidator.ValidateAndThrowAsync(request, cancellationToken);
        }

        var plan = budget.GetBudgetPlanById(ga.BudgetPlanId);
        var planConfig = await _apiService.GetBudgetPlanConfig();

        if (budget.Year != planConfig.CurrentBudgetPlanYear || plan.Quarter != planConfig.CurrentBudgetPlanName ||
            plan.Status != BudgetPlanStatus.Draft)
            throw new ApiException("Grouped activity can't be edited for this budget plan");

        // validate title
        var groupedActivityWithSameTitle = await _gaRepository.FindOneAsync(
            g => g.Id != ga.Id && g.BudgetPlanId == ga.BudgetPlanId && g.Title == request.Title);
        if (groupedActivityWithSameTitle != null)
            throw new ApiException(ErrorMessages.GroupedActivityWithSameTitleAlreadyExists);


        var netPlannedAmount = await _currencyService.CalculateNetPlannedAmount(budget.Year, request.PlannedAmount,
            request.LocalCurrency, request.PlannedSponsorship);
        UpdateGaValues(request, netPlannedAmount, ga);
        await _gaRepository.UpdateAsync(ga);

        return ga;
    }

    private static void UpdateGaValues(EditGroupedActivityToBudgetRequest request, double netPlannedAmount,
        GroupedActivity ga)
    {
        ga.Title = request.Title;
        ga.Quarter = request.Quarter;
        ga.PlannedAmount = request.PlannedAmount;
        ga.NetPlannedAmount = netPlannedAmount;
        ga.PlannedSponsorship = request.PlannedSponsorship;
        ga.LocalCurrency = request.LocalCurrency;
        ga.Comment = request.Comment;
        ga.GlobalRegion = request.GlobalRegion;
        ga.SubRegion = request.SubRegion;
        ga.Alliance = request.Alliance;
        ga.Vendors = request.Vendors;
        ga.ChannelDetails = request.ChannelDetails;
        ga.AdService = request.AdService;
        ga.ExecutionTeam = request.ExecutionTeam;
        ga.ContentType = request.ContentType;
        ga.GlobalProgram = request.GlobalProgram;
        ga.Management = request.Management;
        ga.Team = request.Team;
        ga.Segments = request.Segments;
        ga.Campaigns = request.Campaigns;
    }
}
