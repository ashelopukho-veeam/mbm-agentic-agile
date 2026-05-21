using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TreeItem = mbt.webapi.Domain.Entities.TreeItem;

namespace mbt.webapi.Endpoints.Metadata;

public class ListItems : EndpointBaseAsync.WithoutRequest.WithActionResult<List<TreeItem>>
{
    private readonly IMetadataService _metadataService;

    public ListItems(IMetadataService metadataService)
    {
        _metadataService = metadataService;
    }


    [HttpGet("api/metadata")]
    [SwaggerOperation(
        Summary = "List metadata",
        Description = "List metadata",
        OperationId = "Metadata.List",
        Tags = new[] { "Metadata" })]
    public override async Task<ActionResult<List<TreeItem>>> HandleAsync(
        CancellationToken cancellationToken = new())
    {
        var treeItems = await _metadataService.Get();

        return treeItems;
    }
}
