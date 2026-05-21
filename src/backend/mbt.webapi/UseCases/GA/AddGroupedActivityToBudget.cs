using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using MediatR;

namespace mbt.webapi.UseCases.GA;

[PublicAPI]
public class AddGroupedActivityToBudgetRequest : IRequest<GroupedActivity>, IWithPaidMediaData
{
    public string Title { get; set; }
    public string BudgetId { get; set; }

    public int Quarter { get; set; }
    public double PlannedAmount { get; set; }

    public double PlannedSponsorship { get; set; }

    public string LocalCurrency { get; set; }
    public string Comment { get; set; }
    public string GlobalRegion { get; set; }

    public string SubRegion { get; set; }
    public string Alliance { get; set; }
    public List<string> Vendors { get; set; }
    public string ChannelDetails { get; set; }
    public string AdService { get; set; }
    public string ExecutionTeam { get; set; }
    public string ContentType { get; set; }
    public string GlobalProgram { get; set; }
    public string Management { get; set; }
    public string Team { get; set; }

    public List<TitleNumberValuePair> Segments { get; set; }
    public List<TitleNumberValuePair> Campaigns { get; set; }
}

public class AddGroupedActivityToBudgetRequestValidator : AbstractValidator<AddGroupedActivityToBudgetRequest>
{
    public AddGroupedActivityToBudgetRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(ValidationRulesConstants.TitleMaxLength);
        RuleFor(x => x.BudgetId).IsObjectId();
        RuleFor(x => x.Quarter).InclusiveBetween(1, 4);
        RuleFor(x => x.PlannedAmount).GreaterThan(0);
        RuleFor(x => x.PlannedSponsorship).GreaterThanOrEqualTo(0);
        RuleFor(x => x.LocalCurrency).NotEmpty();
        RuleFor(x => x.GlobalRegion).NotEmpty();
        RuleFor(x => x.SubRegion).NotEmpty();
        RuleFor(x => x.Segments)
            .IsValidPercentageDistribution();
        RuleFor(x => x.Campaigns)
            .IsValidPercentageDistribution();
        RuleFor(x => x.Comment)
            .MaximumLength(ValidationRulesConstants.MaxCommentLength);
    }
}

public class
    AddGroupedActivityToBudgetRequestHandler : IRequestHandler<AddGroupedActivityToBudgetRequest, GroupedActivity>
{
    private readonly IApiService _apiService;
    private readonly IDbBaseRepository<Budget> _budgetRepository;
    private readonly ICurrencyService _currencyService;
    private readonly IDbBaseRepository<GroupedActivity> _gaRepository;
    private readonly IValidator<IWithPaidMediaData> _paidMediaFieldsValidator;

    public AddGroupedActivityToBudgetRequestHandler(IDbBaseRepository<Budget> budgetRepository,
        IDbBaseRepository<GroupedActivity> gaRepository, IApiService apiService, ICurrencyService currencyService,
        IValidator<IWithPaidMediaData> paidMediaFieldsValidator)
    {
        _budgetRepository = budgetRepository;
        _gaRepository = gaRepository;
        _apiService = apiService;
        _currencyService = currencyService;
        _paidMediaFieldsValidator = paidMediaFieldsValidator;
    }

    public async Task<GroupedActivity> Handle(AddGroupedActivityToBudgetRequest request,
        CancellationToken cancellationToken)
    {
        var budget = await _budgetRepository.GetAsync(request.BudgetId);
        if (budget == null)
            throw new ApiException(ErrorMessages.BudgetNotFound(request.BudgetId));

        if (budget.IsPaidMedia)
        {
            await _paidMediaFieldsValidator.ValidateAndThrowAsync(request, cancellationToken);
        }

        var planConfig = await _apiService.GetBudgetPlanConfig();

        if (budget.Year != planConfig.CurrentBudgetPlanYear)
            throw new ApiException("Budget year is not the same as the current budget plan year");

        var plan = budget.GetPlanByQuarter(planConfig.CurrentBudgetPlanName);
        if (plan == null)
            throw new ApiException("Can't find budget plan for the current budget plan year");

        if (plan.Status != BudgetPlanStatus.Draft)
            throw new ApiException("Budget plan is not in Draft status");

        var netPlannedAmount = await _currencyService.CalculateNetPlannedAmount(budget.Year, request.PlannedAmount,
            request.LocalCurrency, request.PlannedSponsorship);

        var groupedActivity = new GroupedActivity
        {
            Title = request.Title,
            BudgetId = request.BudgetId,
            BudgetPlanId = plan.Id,
            Quarter = request.Quarter,
            PlannedAmount = request.PlannedAmount,
            PlannedSponsorship = request.PlannedSponsorship,
            NetPlannedAmount = netPlannedAmount,
            LocalCurrency = request.LocalCurrency,
            Comment = request.Comment,
            GlobalRegion = request.GlobalRegion,
            SubRegion = request.SubRegion,
            Alliance = request.Alliance,
            Vendors = request.Vendors,
            ChannelDetails = request.ChannelDetails,
            AdService = request.AdService,
            ExecutionTeam = request.ExecutionTeam,
            ContentType = request.ContentType,
            GlobalProgram = request.GlobalProgram,
            Management = request.Management,
            Team = request.Team,
            Segments = request.Segments,
            Campaigns = request.Campaigns
        };

        // validate title
        var groupedActivityWithSameTitle = await _gaRepository.FindOneAsync(
            g => g.Title == groupedActivity.Title && g.BudgetPlanId == groupedActivity.BudgetPlanId);
        if (groupedActivityWithSameTitle != null)
            throw new ApiException(ErrorMessages.GroupedActivityWithSameTitleAlreadyExists);

        await _gaRepository.CreateAsync(groupedActivity);

        return groupedActivity;
    }
}
