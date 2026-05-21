using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Endpoints.PaidMediaSet.dto;
using mbt.webapi.Repositories;
using mbt.webapi.UseCases.PaidMediaSet;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.PaidMediaSet;

public class GetById : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithActionResult<PaidMediaSetDto>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IPaidMediaSetRepository _paidMediaSetRepository;
    private readonly IValidator<ObjectIdRequest> _validator;

    public GetById(IMediator mediator, IValidator<ObjectIdRequest> validator, IMapper mapper,
        IPaidMediaSetRepository paidMediaSetRepository)
    {
        _mediator = mediator;
        _validator = validator;
        _mapper = mapper;
        _paidMediaSetRepository = paidMediaSetRepository;
    }

    [Authorize(Roles = AppRoles.PaidMediaRolePolicy)]
    [HttpGet("api/paidMediaSet/{Id}")]
    [SwaggerOperation(
        Summary = "Get a Paid Media Set by Id",
        Description = "Get a Paid Media Set by Id",
        OperationId = "PaidMediaSet.GetById",
        Tags = new[] { "PaidMediaSet" })]
    public override async Task<ActionResult<PaidMediaSetDto>> HandleAsync(
        [FromRoute] ObjectIdRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var paidMediaSetWithLinkedItem = await _paidMediaSetRepository.GetByIdExpanded(request.Id);

        if (paidMediaSetWithLinkedItem == null)
        {
            return NotFound();
        }

        return _mapper.Map<PaidMediaSetWithLinkedItem, PaidMediaSetDto>(paidMediaSetWithLinkedItem);
    }
}
