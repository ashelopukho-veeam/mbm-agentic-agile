using System.Threading;
using System.Threading.Tasks;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using MediatR;

namespace mbt.webapi.UseCases.BudgetStructure.Commands;

public class DeleteBudgetStructureCommand : IRequest<CommandResult>
{
    public string Id { get; init; }
}

public class
    DeleteBudgetStructureCommandHandler : IRequestHandler<DeleteBudgetStructureCommand,
    CommandResult>
{
    private readonly IBudgetRepository _budgetRepository;

    public DeleteBudgetStructureCommandHandler(IBudgetRepository budgetRepository)
    {
        _budgetRepository = budgetRepository;
    }

    public async Task<CommandResult> Handle(DeleteBudgetStructureCommand request,
        CancellationToken cancellationToken)
    {
        var result = new CommandResult();

        var budgetToDelete = await _budgetRepository.GetAsync(request.Id);
        if (budgetToDelete == null)
        {
            result.IsSuccess = false;
            result.IsNotFound = true;
            result.Message = $"Budget with Id {request.Id} not found";
            return result;
        }

        if (budgetToDelete.Status != BudgetStatus.InProgress)
        {
            result.IsSuccess = false;
            result.Message = $"Budget with Id {request.Id} cannot be deleted because it is not in progress";
            return result;
        }


        await _budgetRepository.RemoveAsync(budgetToDelete.Id);

        result.IsSuccess = true;
        result.Message = $"Budget with Id {request.Id} deleted successfully";
        return result;
    }
}
