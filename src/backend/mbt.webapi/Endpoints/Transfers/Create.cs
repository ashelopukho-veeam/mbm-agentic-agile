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

public class Create : EndpointBaseAsync.WithRequest<CreateTransferRequest>.WithActionResult<TransferDto>
{
    private readonly IMapper _mapper;
    private readonly IValidator<CreateTransferRequest> _validator;
    private readonly IMediator _mediator;


    public Create(IMapper mapper, IValidator<CreateTransferRequest> validator, IMediator mediator)
    {
        _mapper = mapper;
        _validator = validator;
        _mediator = mediator;
    }

    [HttpPost("api/transfers")]
    [Authorize(Roles = $"{AppRoles.AdminPolicy},{AppRoles.Designers},{AppRoles.Contributors}")]
    [SwaggerOperation(
        Summary = "Create transfer",
        Description = "Create transfer",
        OperationId = "Transfers.Create",
        Tags = new[] { "Transfers" })]
    public override async Task<ActionResult<TransferDto>> HandleAsync(CreateTransferRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var transfer = await _mediator.Send(new CreateTransferCommand(request), cancellationToken);

        var result = _mapper.Map<TransferDto>(transfer);

        return result;
    }
}
