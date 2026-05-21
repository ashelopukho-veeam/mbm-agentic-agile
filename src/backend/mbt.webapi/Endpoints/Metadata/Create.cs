using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.Metadata;

[Authorize(Roles = AppRoles.AdminPolicy)]
public class Create : EndpointBaseAsync.WithRequest<CreateTreeItemRequest>.WithActionResult<TreeItem>
{
    private readonly IMetadataService _metadataService;
    private readonly IValidator<CreateTreeItemRequest> _validator;

    public Create(IMetadataService metadataService, IValidator<CreateTreeItemRequest> validator)
    {
        _metadataService = metadataService;
        _validator = validator;
    }


    [HttpPost("api/metadata")]
    [SwaggerOperation(
        Summary = "Create a metadata item",
        Description = "Create a metadata item",
        OperationId = "Metadata.Create",
        Tags = new[] { "Metadata" })]
    public override async Task<ActionResult<TreeItem>> HandleAsync([FromBody] CreateTreeItemRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var newTreeItem = new TreeItem
        {
            Title = request.Title,
            ParentId = request.ParentId,
            Value = request.Value
        };

        await _metadataService.Create(newTreeItem);

        return Ok(newTreeItem);
    }
}


