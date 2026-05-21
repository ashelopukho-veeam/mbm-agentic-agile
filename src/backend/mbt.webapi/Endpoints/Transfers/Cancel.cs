using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.UseCases;
using mbt.webapi.UseCases.Transfers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ApiException = Microsoft.Kiota.Abstractions.ApiException;

namespace mbt.webapi.Endpoints.Transfers;

public class Cancel : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithActionResult
{
    private readonly IMediator _mediator;
    private readonly IValidator<ObjectIdRequest> _validator;

    public Cancel(IMediator mediator, IValidator<ObjectIdRequest> validator)
    {
        _mediator = mediator;
        _validator = validator;
    }

    [HttpPost("api/transfers/{Id}/cancel")]
    [Authorize(Roles = $"{AppRoles.AdminPolicy},{AppRoles.Designers},{AppRoles.Contributors}")]
    [SwaggerOperation(
        Summary = "Cancel a transfer",
        Description = "Cancel a transfer",
        OperationId = "Transfers.Cancel",
        Tags = new[] { "Transfers" })]
    public override async Task<ActionResult> HandleAsync([FromRoute] ObjectIdRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var commandResult = await _mediator.Send(new CancelTransferCommand(request.Id), cancellationToken);

        return commandResult.Status switch
        {
            CommandResultStatus.NotFound => NotFound(),
            CommandResultStatus.Failure => throw new ApiException(commandResult.Message),
            _ => Ok()
        };
    }
}
