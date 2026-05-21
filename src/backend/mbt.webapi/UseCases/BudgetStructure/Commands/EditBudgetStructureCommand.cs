using System.Threading;
using System.Threading.Tasks;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Endpoints.BudgetStructure.dto;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Budgets;
using MediatR;

namespace mbt.webapi.UseCases.BudgetStructure.Commands;

public record EditBudgetStructureCommand(EditBudgetStructureRequest Request) : IRequest<CommandResult<Budget>>;

public class EditBudgetStructureCommandHandler : IRequestHandler<EditBudgetStructureCommand, CommandResult<Budget>>
{
    private readonly IBudgetRepository _budgetsRepository;
    private readonly IBudgetConstructionService _budgetConstructionService;
    private readonly IBudgetValidationService _budgetValidationService;

    public EditBudgetStructureCommandHandler(IBudgetRepository budgetsRepository,
        IBudgetConstructionService budgetConstructionService, IBudgetValidationService budgetValidationService)
    {
        _budgetsRepository = budgetsRepository;
        _budgetConstructionService = budgetConstructionService;
        _budgetValidationService = budgetValidationService;
    }

    public async Task<CommandResult<Budget>> Handle(EditBudgetStructureCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;

        var originalBudget = await _budgetsRepository.GetAsync(request.Id);
        if (originalBudget == null)
            return CommandResult<Budget>.NotFound(ErrorMessages.BudgetNotFound(request.Id));

        if (originalBudget.Status != BudgetStatus.InProgress)
            throw new ApiException(ErrorMessages.BudgetEditWrongStatus);

        originalBudget.Title = await _budgetConstructionService.BuildBudgetTitle(
            request.Level1,
            request.Level2,
            request.Level3,
            request.CostCenter,
            request.Year);

        originalBudget.BudgetType = request.BudgetType;
        originalBudget.Level1 = request.Level1;
        originalBudget.Level2 = request.Level2;
        originalBudget.Level3 = request.Level3;
        originalBudget.CostCenter = request.CostCenter;
        originalBudget.Year = request.Year;
        originalBudget.ParentManagerId = request.ParentManagerId.ToString();
        originalBudget.OwnerId = request.OwnerId.ToString();
        originalBudget.ManagerId = request.ManagerId.ToString();
        originalBudget.IsPaidMedia = request.IsPaidMedia;

        await _budgetValidationService.ValidateBudgetTitle(originalBudget);
        await _budgetsRepository.UpdateAsync(originalBudget);

        return CommandResult<Budget>.Success(originalBudget);
    }
}
