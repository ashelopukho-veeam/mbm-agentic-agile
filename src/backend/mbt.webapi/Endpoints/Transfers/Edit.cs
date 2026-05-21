using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Endpoints.Transfers.dto;
using mbt.webapi.UseCases.Transfers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.Transfers;

public class Edit : EndpointBaseAsync.WithRequest<EditTransferRequest>.WithActionResult<TransferDto>
{
    private readonly IMapper _mapper;
    private readonly IValidator<EditTransferRequest> _validator;
    private readonly IMediator _mediator;

    public Edit(IMapper mapper,
        IValidator<EditTransferRequest> validator, IMediator mediator)
    {
        _mapper = mapper;
        _validator = validator;
        _mediator = mediator;
    }

    [HttpPut(TransferRoutes.Update)]
    [Authorize(Roles = $"{AppRoles.AdminPolicy},{AppRoles.Designers}, {AppRoles.Contributors}")]
    [SwaggerOperation(
        Summary = "Edit a transfer",
        Description = "Edit a transfer",
        OperationId = "Transfers.Edit",
        Tags = new[] { "Transfers" })]
    public override async Task<ActionResult<TransferDto>> HandleAsync(EditTransferRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var transfer = await _mediator.Send(new EditTransferCommand(request), cancellationToken);

        return _mapper.Map<TransferDto>(transfer);
    }
}
