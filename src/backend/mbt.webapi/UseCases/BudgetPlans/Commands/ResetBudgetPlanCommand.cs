using System.Threading;
using System.Threading.Tasks;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Services.Interfaces;
using MediatR;

namespace mbt.webapi.UseCases.BudgetPlans.Commands;

public class ResetBudgetPlanCommand : IRequest<BudgetPlan>
{
    public string BudgetPlanId { get; set; }
}

public class ResetBudgetPlanCommandHandler : IRequestHandler<ResetBudgetPlanCommand, BudgetPlan>
{
    private readonly IBudgetService _budgetService;
    private readonly IChatService _chatService;
    private readonly ICurrentUserContext _currentUserContext;

    public ResetBudgetPlanCommandHandler(IBudgetService budgetService, IChatService chatService,
        ICurrentUserContext currentUserContext)
    {
        _budgetService = budgetService;
        _chatService = chatService;
        _currentUserContext = currentUserContext;
    }

    public async Task<BudgetPlan> Handle(ResetBudgetPlanCommand request, CancellationToken cancellationToken)
    {
        var budgetPlanId = request.BudgetPlanId;
        var budget = await _budgetService.GetBudgetByPlanId(budgetPlanId);

        var plan = budget.GetBudgetPlanById(budgetPlanId);
        if (plan.Status != BudgetPlanStatus.Approved)
            throw new ApiException("Budget plan not approved");

        plan.Status = BudgetPlanStatus.Draft;
        await _budgetService.UpdateAsync(budget);

        await _chatService.AddSystemChatMessageAsync(budget.Id,
            "Reset to Draft by " + _currentUserContext.UserName);

        return plan;
    }
}