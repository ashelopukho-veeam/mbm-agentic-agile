using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Endpoints.BudgetStructure.dto;
using mbt.webapi.UseCases.BudgetStructure.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.BudgetStructure;

public class Edit : EndpointBaseAsync.WithRequest<EditBudgetStructureRequest>.WithActionResult<BudgetStructureDto>
{
    private readonly IValidator<EditBudgetStructureRequest> _validator;
    private readonly IMediator _mediator;

    public Edit(IValidator<EditBudgetStructureRequest> validator, IMediator mediator)
    {
        _validator = validator;
        _mediator = mediator;
    }

    [HttpPut("api/budgetStructure")]
    [Authorize(Roles = AppRoles.AdminPolicy)]
    [SwaggerOperation(
        Summary = "Edit budget structure",
        Description = "Edit budget structure",
        OperationId = "BudgetStructure.Edit",
        Tags = new[] { "BudgetStructure" })]
    public override async Task<ActionResult<BudgetStructureDto>> HandleAsync(EditBudgetStructureRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var originalBudget = await _mediator.Send(new EditBudgetStructureCommand(request), cancellationToken);

        return Ok(originalBudget.Data);
    }
}
