using System.Threading;
using System.Threading.Tasks;
using mbt.webapi.BuiltIn;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using mbt.webapi.Shared;
using MediatR;

namespace mbt.webapi.UseCases.BudgetStructure.Commands;

public class ApproveBudgetStructureCommand : IRequest<Budget>
{
    public ApproveBudgetStructureCommand(string id)
    {
        Id = id;
    }

    public string Id { get; }
}

public class ApproveBudgetStructureHandler : IRequestHandler<ApproveBudgetStructureCommand, Budget>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IChatService _chatService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IApiService _apiService;

    public ApproveBudgetStructureHandler(
        IBudgetRepository budgetRepository,
        IChatService chatService,
        ICurrentUserContext currentUserContext, IApiService apiService)
    {
        _budgetRepository = budgetRepository;
        _chatService = chatService;
        _currentUserContext = currentUserContext;
        _apiService = apiService;
    }

    public async Task<Budget> Handle(ApproveBudgetStructureCommand command, CancellationToken cancellationToken)
    {
        var budget = await _budgetRepository.GetAsync(command.Id);

        if (budget == null)
            throw new ApiException(ErrorMessages.BudgetNotFound(command.Id));

        if (budget.Status != BudgetStatus.InProgress &&
            budget.Status != BudgetStatus.ApprovedPlaceholder)
            throw new ApiException(ErrorMessages.BudgetStructureNotInProgressOrApprovedPlaceholder);

        budget.Status = BudgetStatus.Approved;
        budget.UseInCoupa = true;
        budget.UseInTableau = true;

        await EnsureBudgetPlans(budget);

        await _budgetRepository.UpdateAsync(budget);

        await _chatService.AddSystemChatMessageAsync(budget.Id,
            "Budget structure approved by: " + _currentUserContext.UserName);

        return budget;
    }

    private async Task EnsureBudgetPlans(Budget budget)
    {
        if (budget.Plans.Count > 0)
            return;

        var activeForecastPeriod = await _apiService.GetActiveForecastPeriod();

        for (var i = 0; i < BuiltInConstants.ForecastsPerYear; i++)
        {
            var period = new Period(budget.Year, i + 1);

            budget.Plans.Add(new BudgetPlan()
            {
                Quarter = period.PlanName,
                Status = period < activeForecastPeriod ? BudgetPlanStatus.Final : BudgetPlanStatus.Draft
            });
        }
    }
}
