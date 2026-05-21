using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.BuiltIn;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.PaidMediaSet;

public class Delete : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithoutResult
{
    private readonly IPaidMediaSetService _paidMediaSetService;

    public Delete(IPaidMediaSetService paidMediaSetService)
    {
        _paidMediaSetService = paidMediaSetService;
    }

    [Authorize(Roles = AppRoles.PaidMediaRolePolicy)]
    [HttpDelete("api/paidMediaSet/{Id}")]
    [SwaggerOperation(
        Summary = "Deletes a Paid Media set",
        Description = "Deletes a Paid Media set",
        OperationId = "PaidMediaSet.Delete",
        Tags = new[] { "PaidMediaSet" })]
    public override async Task<ActionResult> HandleAsync([FromRoute] ObjectIdRequest request,
        CancellationToken cancellationToken = new())
    {
        var itemToDelete = await _paidMediaSetService.GetAsync(request.Id);
        if (itemToDelete is null) return NotFound();

        await _paidMediaSetService.RemoveAsync(itemToDelete);

        return Ok();
    }
}
