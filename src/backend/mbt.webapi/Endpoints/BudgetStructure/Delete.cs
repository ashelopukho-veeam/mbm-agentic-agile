using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.UseCases.BudgetStructure.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.BudgetStructure;

public class Delete : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithActionResult
{
    private readonly IMediator _mediator;
    private readonly IValidator<ObjectIdRequest> _validator;

    public Delete(IMediator mediator, IValidator<ObjectIdRequest> validator)
    {
        _mediator = mediator;
        _validator = validator;
    }

    [HttpDelete("api/budgetStructure/{Id}")]
    [Authorize(Roles = AppRoles.AdminPolicy)]
    [SwaggerOperation(
        Summary = "Delete a budget structure by Id",
        Description = "Delete a budget structure by Id",
        OperationId = "BudgetStructure.Delete",
        Tags = new[] { "BudgetStructure" })]
    public override async Task<ActionResult> HandleAsync([FromRoute] ObjectIdRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var result = await _mediator.Send(new DeleteBudgetStructureCommand { Id = request.Id }, cancellationToken);

        if (result.IsNotFound)
            return NotFound(result.Message);

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok();
    }
}
