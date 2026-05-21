using System.Collections.Generic;
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

namespace mbt.webapi.Endpoints.PaidMediaSet;

public class GePaidMediaSetDetails :
    EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithActionResult<List<PaidMediaDetails>>
{
    private readonly IPaidMediaSetRepository _paidMediaSetRepository;
    private readonly IValidator<ObjectIdRequest> _validator;

    public GePaidMediaSetDetails(IValidator<ObjectIdRequest> validator, IPaidMediaSetRepository paidMediaSetRepository)
    {
        _validator = validator;
        _paidMediaSetRepository = paidMediaSetRepository;
    }

    [Authorize(Roles = AppRoles.PaidMediaRolePolicy)]
    [HttpGet("api/paidMediaSet/{Id}/details")]
    [SwaggerOperation(
        Summary = "Get a Paid Media Set details by Id",
        Description = "Get a Paid Media Set details by Id",
        OperationId = "PaidMediaSet.GetDetailsById",
        Tags = new[] { "PaidMediaSet" })]
    public override async Task<ActionResult<List<PaidMediaDetails>>> HandleAsync([FromRoute] ObjectIdRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);

        var paidMediaSet = await _paidMediaSetRepository.GetByIdExpanded(request.Id);

        if (paidMediaSet == null)
        {
            return NotFound();
        }

        return paidMediaSet.Details;
    }
}
