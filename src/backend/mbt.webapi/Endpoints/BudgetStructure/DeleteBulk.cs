using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Services;
using mbt.webapi.UseCases.BudgetStructure.Commands;
using mbt.webapi.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.BudgetStructure;

public class DeleteBulk : EndpointBaseAsync.WithRequest<BulkIdsRequest>.WithResult<BulkOperationResult>
{
    private readonly IValidator<BulkIdsRequest> _validator;
    private readonly IMediator _mediator;

    public DeleteBulk(IValidator<BulkIdsRequest> validator, IMediator mediator)
    {
        _validator = validator;
        _mediator = mediator;
    }

    [HttpDelete("api/budgetStructure")]
    [Authorize(Roles = AppRoles.AdminPolicy)]
    [SwaggerOperation(
        Summary = "Delete budget structures",
        Description = "Delete budget structures",
        OperationId = "BudgetStructure.BulkDelete",
        Tags = new[] { "BudgetStructure" })]
    public override async Task<BulkOperationResult> HandleAsync(BulkIdsRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var result = await BulkOperation.Run(request.Ids,
            async id => { await _mediator.Send(new DeleteBudgetStructureCommand { Id = id }, cancellationToken); },
            "Delete budget structures");

        return result;
    }
}
