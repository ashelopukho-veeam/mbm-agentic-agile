using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Endpoints.BudgetStructure.dto;
using mbt.webapi.Services.Interfaces;
using mbt.webapi.UseCases.BudgetStructure.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.BudgetStructure;

public class Create : EndpointBaseAsync.WithRequest<CreateBudgetStructureRequest>.WithActionResult<BudgetStructureDto>
{
    private readonly IValidator<CreateBudgetStructureRequest> _validator;
    private readonly IMediator _mediator;

    public Create(IValidator<CreateBudgetStructureRequest> validator, IMediator mediator)
    {
        _validator = validator;
        _mediator = mediator;
    }

    [HttpPost("api/budgetStructure")]
    [Authorize(Roles = AppRoles.AdminPolicy)]
    [SwaggerOperation(
        Summary = "Create budget structure",
        Description = "Create budget structure",
        OperationId = "BudgetStructure.Create",
        Tags = new[] { "BudgetStructure" })]
    public override async Task<ActionResult<BudgetStructureDto>> HandleAsync(CreateBudgetStructureRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var result = await _mediator.Send(new CreateBudgetStructureCommand(request), cancellationToken);

        return BudgetStructureDto.FromBudget(result.Data);
    }
}
