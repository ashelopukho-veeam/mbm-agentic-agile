using System.Threading;
using System.Threading.Tasks;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using MediatR;

namespace mbt.webapi.UseCases.BudgetStructure.Commands;

public record ReactivateBudgetStructureCommand(string Id) : IRequest<CommandResult<Budget>>;

public class
    ReactivateBudgetStructureCommandHandler : IRequestHandler<ReactivateBudgetStructureCommand, CommandResult<Budget>>
{
    private readonly IBudgetRepository _budgetRepository;

    public ReactivateBudgetStructureCommandHandler(IBudgetRepository budgetRepository)
    {
        _budgetRepository = budgetRepository;
    }

    public async Task<CommandResult<Budget>> Handle(ReactivateBudgetStructureCommand request,
        CancellationToken cancellationToken)
    {
        var budget = await _budgetRepository.GetAsync(request.Id);

        if (budget == null)
            throw new ApiException(ErrorMessages.BudgetNotFound(request.Id));

        if (budget.Status != BudgetStatus.Inactive)
            throw new ApiException(ErrorMessages.BudgetIsNotInactive(budget.Title));

        budget.Status = BudgetStatus.Approved;
        budget.UseInCoupa = true;
        budget.UseInTableau = true;
        await _budgetRepository.UpdateAsync(budget);

        return CommandResult<Budget>.Success(budget);
    }
}
