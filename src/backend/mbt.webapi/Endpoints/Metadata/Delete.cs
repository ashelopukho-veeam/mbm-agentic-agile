using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.UseCases.Metadata;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.Metadata;

[Authorize(Roles = AppRoles.AdminPolicy)]
public class Delete : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithActionResult
{
    private readonly IMediator _mediator;
    private readonly IValidator<ObjectIdRequest> _validator;

    public Delete(IMediator mediator, IValidator<ObjectIdRequest> validator)
    {
        _mediator = mediator;
        _validator = validator;
    }

    [HttpDelete("api/metadata/{Id}")]
    [SwaggerOperation(
        Summary = "Delete a metadata item",
        Description = "Delete a metadata item",
        OperationId = "Metadata.Delete",
        Tags = new[] { "Metadata" })]
    public override async Task<ActionResult> HandleAsync([FromRoute] ObjectIdRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);

        await _mediator.Send(new RemoveMetadataTreeItemCommand { Id = request.Id }, cancellationToken);

        return Ok();
    }
}
