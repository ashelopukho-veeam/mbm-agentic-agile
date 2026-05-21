using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Endpoints.BudgetStructure.dto;
using mbt.webapi.Services;
using mbt.webapi.UseCases.BudgetStructure.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.BudgetStructure;

public class BulkEdit : EndpointBaseAsync
    .WithRequest<BulkEditRequest>
    .WithResult<BulkOperationResult>
{
    private readonly IValidator<BulkEditRequest> _validator;
    private readonly IMediator _mediator;

    public BulkEdit(IValidator<BulkEditRequest> validator, IMediator mediator)
    {
        _validator = validator;
        _mediator = mediator;
    }

    [HttpPost("api/budgetStructure/bulkEdit")]
    [Authorize(Roles = AppRoles.AdminPolicy)]
    [SwaggerOperation(
        Summary = "Bulk edit",
        Description = "Bulk edit",
        OperationId = "BudgetStructure.BulkEdit",
        Tags = new[] { "BudgetStructure" })]
    public override async Task<BulkOperationResult> HandleAsync(BulkEditRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var command = new BulkEditBudgetStructureCommand(request);
        var result = await _mediator.Send(command, cancellationToken);

        return result;
    }
}
