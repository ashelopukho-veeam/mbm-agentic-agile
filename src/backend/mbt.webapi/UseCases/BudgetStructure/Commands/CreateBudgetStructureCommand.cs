using System.Threading;
using System.Threading.Tasks;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Endpoints.BudgetStructure.dto;
using mbt.webapi.Services.Budgets;
using mbt.webapi.Services.Interfaces;
using MediatR;

namespace mbt.webapi.UseCases.BudgetStructure.Commands;

public record CreateBudgetStructureCommand(CreateBudgetStructureRequest Request) : IRequest<CommandResult<Budget>>;

public class CreateBudgetStructureCommandHandler : IRequestHandler<CreateBudgetStructureCommand, CommandResult<Budget>>
{
    private readonly IBudgetService _budgetService;

    public CreateBudgetStructureCommandHandler(
        IBudgetService budgetService)
    {
        _budgetService = budgetService;
    }

    public async Task<CommandResult<Budget>> Handle(CreateBudgetStructureCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;

        var budget = await _budgetService.CreateAsync(request);
        return CommandResult<Budget>.Success(budget);
    }
}
