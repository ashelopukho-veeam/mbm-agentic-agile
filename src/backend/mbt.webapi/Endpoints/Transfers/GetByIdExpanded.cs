using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using mbt.webapi.BuiltIn;
using mbt.webapi.Endpoints.Transfers.dto;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.Transfers;

public class GetByIdExpanded : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithActionResult<TransferExpandedDto>
{
    private readonly IMapper _mapper;
    private readonly ITransfersService _transfersService;

    public GetByIdExpanded(ITransfersService transfersService, IMapper mapper)
    {
        _transfersService = transfersService;
        _mapper = mapper;
    }

    [HttpGet("api/transfers/{Id}/expand")]
    [Authorize(Roles = AppRoles.ViewPolicy)]
    [SwaggerOperation(
        Summary = "Get an expanded transfer by Id",
        Description = "Get an expanded transfer by Id",
        OperationId = "Transfers.GetByIdExpanded",
        Tags = new[] { "Transfers" })]
    public override async Task<ActionResult<TransferExpandedDto>> HandleAsync([FromRoute] ObjectIdRequest request,
        CancellationToken cancellationToken = new())
    {
        var transferExpanded = await _transfersService.GetExpanded(request.Id);
        if (transferExpanded is null) return NotFound();

        var result = _mapper.Map<TransferExpandedDto>(transferExpanded);

        return Ok(result);
    }
}
