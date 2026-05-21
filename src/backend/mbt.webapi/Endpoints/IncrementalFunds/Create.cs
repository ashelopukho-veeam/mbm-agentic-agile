using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Endpoints.IncrementalFunds.dto;
using mbt.webapi.UseCases.IncrementalFunds;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.IncrementalFunds;

[Authorize(Roles = AppRoles.ManageGroupedActivities)]
public class Create : EndpointBaseAsync
    .WithRequest<CreateIncrementalFundRequest>
    .WithActionResult<IncrementalFundDto>
{
    private readonly IMapper _mapper;
    private readonly IValidator<CreateIncrementalFundRequest> _validator;
    private readonly IMediator _mediator;

    public Create(IMapper mapper, IValidator<CreateIncrementalFundRequest> validator, IMediator mediator)
    {
        _mapper = mapper;
        _validator = validator;
        _mediator = mediator;
    }


    [HttpPost("api/incrementalFunds")]
    [Authorize(Roles = $"{AppRoles.AdminPolicy},{AppRoles.Designers}, {AppRoles.Contributors}")]
    [SwaggerOperation(
        Summary = "Create incremental fund",
        Description = "Create incremental fund",
        OperationId = "IncrementalFunds.Create",
        Tags = new[] { "IncrementalFunds" })]
    public override async Task<ActionResult<IncrementalFundDto>> HandleAsync(CreateIncrementalFundRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);


        var newIncrementalFund = await _mediator.Send(
            new CreateIncrementalFundCommand(request), cancellationToken);

        var incrementalFundDto = _mapper.Map<IncrementalFundDto>(newIncrementalFund);

        return Ok(incrementalFundDto);
    }
}
