using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.Metadata;

[Authorize(Roles = AppRoles.AdminPolicy)]
public class Edit : EndpointBaseAsync.WithRequest<EditTreeItemRequest>.WithActionResult<TreeItem>
{
    private readonly IDbBaseRepository<TreeItem> _metadataRepository;
    private readonly IValidator<EditTreeItemRequest> _validator;

    public Edit(IDbBaseRepository<TreeItem> metadataRepository, IValidator<EditTreeItemRequest> validator)
    {
        _metadataRepository = metadataRepository;
        _validator = validator;
    }

    [HttpPut("api/metadata")]
    [SwaggerOperation(
        Summary = "Edit a metadata item",
        Description = "Edit a metadata item",
        OperationId = "Metadata.Edit",
        Tags = new[] { "Metadata" })]
    public override async Task<ActionResult<TreeItem>> HandleAsync(EditTreeItemRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var treeItem =  await _metadataRepository.FindOneAsync(t => t.Id == request.Id);

        if (treeItem == null)
            return NotFound("Metadata item not found");

        treeItem.Title = request.Title;
        treeItem.Value = request.Value;

        await _metadataRepository.UpdateAsync(treeItem);

        return Ok(treeItem);
    }
}
