using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.BuiltIn;
using mbt.webapi.Endpoints.BudgetStructure.dto;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.BudgetStructure;

public class SetUsers : EndpointBaseAsync
    .WithRequest<SetUsersBudgetStructureRequest>
    .WithActionResult<BudgetStructureDto>
{
    private readonly IBudgetRepository _budgetRepository;

    public SetUsers(IBudgetRepository budgetRepository)
    {
        _budgetRepository = budgetRepository;
    }

    [HttpPost("api/budgetStructure/setUsers")]
    [Authorize(Roles = AppRoles.AdminPolicy)]
    [SwaggerOperation(
        Summary = "Set budget structure users",
        Description = "Set budget structure users",
        OperationId = "BudgetStructure.SetUsers",
        Tags = new[] { "BudgetStructure" })]
    public override async Task<ActionResult<BudgetStructureDto>> HandleAsync(SetUsersBudgetStructureRequest request,
        CancellationToken cancellationToken = new())
    {
        //  set only owners and managers
        var originalBudget = await _budgetRepository.GetAsync(request.Id);
        if (originalBudget == null) return NotFound();

        originalBudget.ParentManagerId = request.ParentManagerId.ToString();
        originalBudget.OwnerId = request.OwnerId.ToString();
        originalBudget.ManagerId = request.ManagerId.ToString();

        await _budgetRepository.UpdateAsync(originalBudget);

        return Ok(originalBudget);
    }
}
