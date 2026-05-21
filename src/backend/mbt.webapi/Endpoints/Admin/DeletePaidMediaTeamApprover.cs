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

namespace mbt.webapi.Endpoints.Admin;

[Authorize(Roles = AppRoles.AdminPolicy)]
public class DeletePaidMediaTeamApprover : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithActionResult
{
    private readonly IDbBaseRepository<PaidMediaTeamApprover> _configRepository;
    private readonly IValidator<ObjectIdRequest> _validator;

    public DeletePaidMediaTeamApprover(IDbBaseRepository<PaidMediaTeamApprover> configRepository,
        IValidator<ObjectIdRequest> validator)
    {
        _configRepository = configRepository;
        _validator = validator;
    }


    [HttpDelete(PaidMediaTeamApproverRoutes.Delete)]
    [SwaggerOperation(
        Summary = "Delete Paid media team approvers",
        Description = "Delete paid media team approvers",
        OperationId = "Admin.DeletePaidMediaTeamApprovers",
        Tags = new[] { "Admin" })]
    public override async Task<ActionResult> HandleAsync([FromRoute] ObjectIdRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);

        var item = await _configRepository.GetAsync(request.Id);
        if (item == null)
            return NotFound();

        await _configRepository.RemoveAsync(request.Id);

        return Ok();
    }
}
