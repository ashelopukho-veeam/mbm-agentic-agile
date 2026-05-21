using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Endpoints.Transfers.dto;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.Transfers;

public class
    ListItemsExpanded : EndpointBaseAsync.WithRequest<ListTransfersRequest>.WithResult<List<TransferExpandedDto>>
{
    private readonly IMapper _mapper;
    private readonly ITransfersService _transfersService;
    private readonly IValidator<ListTransfersRequest> _validator;

    public ListItemsExpanded(ITransfersService transfersService, IMapper mapper, IValidator<ListTransfersRequest> validator)
    {
        _transfersService = transfersService;
        _mapper = mapper;
        _validator = validator;
    }

    [HttpGet("api/transfers/expand")]
    [Authorize(Roles = AppRoles.ViewPolicy)]
    [SwaggerOperation(
        Summary = "List expanded transfers",
        Description = "List expanded transfers",
        OperationId = "Transfers.ListExpanded",
        Tags = new[] { "Transfers" })]
    public override async Task<List<TransferExpandedDto>> HandleAsync(
        [FromQuery] ListTransfersRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);

        var transfers = await _transfersService.GetExpanded(request.Year, request.Plan);

        return transfers.Select(_mapper.Map<TransferExpandedDto>).ToList();
    }
}
