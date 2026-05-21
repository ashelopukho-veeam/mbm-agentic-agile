using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using MediatR;

namespace mbt.webapi.UseCases.BudgetPlans;

public record GetBudgetPlanByIdRequest(string BudgetPlanId) : IRequest<GetBudgetPlanByIdResponse>;

public class GetBudgetPlanByIdRequestHandler : IRequestHandler<GetBudgetPlanByIdRequest, GetBudgetPlanByIdResponse>
{
    private readonly IBudgetRepository _budgetRepository;

    public GetBudgetPlanByIdRequestHandler(IBudgetRepository budgetRepository)
    {
        _budgetRepository = budgetRepository;
    }

    public async Task<GetBudgetPlanByIdResponse> Handle(GetBudgetPlanByIdRequest request,
        CancellationToken cancellationToken)
    {
        var budget = await _budgetRepository.FindOneAsync(b => b.Plans.Any(p => p.Id == request.BudgetPlanId));
        if (budget == null)
        {
            return new GetBudgetPlanByIdResponse(true, null);
        }

        var budgetPlan = budget.Plans.First(p => p.Id == request.BudgetPlanId);

        var response = new BudgetPlanResponse(
            budgetPlan.Id,
            budgetPlan.Quarter,
            budgetPlan.Q1,
            budgetPlan.Q2,
            budgetPlan.Q3,
            budgetPlan.Q4,
            budgetPlan.Segments,
            budgetPlan.Campaigns,
            budgetPlan.Status,
            budget.Id,
            budget.Title);

        return new GetBudgetPlanByIdResponse(false, response);
    }
}

public record GetBudgetPlanByIdResponse(bool IsNotFound, BudgetPlanResponse Response);

[PublicAPI]
public record BudgetPlanResponse(
    string Id,
    string Quarter,
    double Q1,
    double Q2,
    double Q3,
    double Q4,
    List<TitleNumberValuePair> Segments,
    List<TitleNumberValuePair> Campaigns,
    string Status,
    string BudgetId,
    string BudgetTitle);
