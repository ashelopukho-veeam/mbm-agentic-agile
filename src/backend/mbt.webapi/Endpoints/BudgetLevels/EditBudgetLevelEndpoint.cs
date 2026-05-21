using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.BudgetLevels;

public class
    EditBudgetLevelEndpoint : EndpointBaseAsync.WithRequest<EditBudgetLevelRequest>.WithActionResult<BudgetLevelDto>
{
    private readonly IDbBaseRepository<BudgetLevel> _budgetLevelRepository;
    private readonly IValidator<EditBudgetLevelRequest> _validator;

    public EditBudgetLevelEndpoint(IValidator<EditBudgetLevelRequest> validator,
        IDbBaseRepository<BudgetLevel> budgetLevelRepository)
    {
        _validator = validator;
        _budgetLevelRepository = budgetLevelRepository;
    }

    [Authorize(Roles = AppRoles.AdminPolicy)]
    [HttpPut("api/levels/{Id}")]
    [SwaggerOperation(
        Summary = "Edit budget level",
        Description = "Edit budget level",
        OperationId = "BudgetLevels.Edit",
        Tags = new[] { "BudgetLevels" })]
    public override async Task<ActionResult<BudgetLevelDto>> HandleAsync([FromRoute] EditBudgetLevelRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var budgetLevel = await _budgetLevelRepository.GetAsync(request.Id);

        if (budgetLevel == null)
            return NotFound();

        budgetLevel.Level = request.Item.Level;
        budgetLevel.ShortTitle = request.Item.ShortTitle;
        budgetLevel.Title = request.Item.Title;

        await _budgetLevelRepository.UpdateAsync(budgetLevel);

        return BudgetLevelDto.FromBudgetLevel(budgetLevel);
    }
}
