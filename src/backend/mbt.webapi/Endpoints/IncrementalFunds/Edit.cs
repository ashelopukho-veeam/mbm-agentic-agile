using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Endpoints.IncrementalFunds.dto;
using mbt.webapi.Services.Interfaces;
using mbt.webapi.UseCases.IncrementalFunds;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using IncrementalFundStatus = mbt.webapi.Domain.Entities.IncrementalFundStatus;

namespace mbt.webapi.Endpoints.IncrementalFunds;

public class Edit : EndpointBaseAsync.WithRequest<UpdateIncrementalFundRequest>.WithActionResult<IncrementalFundDto>
{
    private readonly IIncrementalFundsService _incrementalFundsService;
    private readonly IMapper _mapper;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IValidator<UpdateIncrementalFundRequest> _validator;
    private readonly IMediator _mediator;


    public Edit(IIncrementalFundsService incrementalFundsService, IMapper mapper,
        ICurrentUserContext currentUserContext, IValidator<UpdateIncrementalFundRequest> validator, IMediator mediator)
    {
        _incrementalFundsService = incrementalFundsService;
        _mapper = mapper;
        _currentUserContext = currentUserContext;
        _validator = validator;
        _mediator = mediator;
    }

    [HttpPut("api/incrementalFunds")]
    [Authorize(Roles = $"{AppRoles.AdminPolicy},{AppRoles.Designers},{AppRoles.Contributors}")]
    [SwaggerOperation(
        Summary = "Edit an Incremental Fund",
        Description = "Edit an Incremental Fund",
        OperationId = "IncrementalFunds.Edit",
        Tags = new[] { "IncrementalFunds" })]
    public override async Task<ActionResult<IncrementalFundDto>> HandleAsync(UpdateIncrementalFundRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var incrementalFund = await _mediator.Send(new EditIncrementalFundCommand(request), cancellationToken);



        return _mapper.Map<IncrementalFundDto>(incrementalFund);
    }
}
