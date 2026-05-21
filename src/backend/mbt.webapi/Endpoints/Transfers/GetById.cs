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

public class GetById : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithActionResult<TransferDto>
{
    private readonly IMapper _mapper;
    private readonly ITransfersService _transfersService;

    public GetById(ITransfersService transfersService, IMapper mapper)
    {
        _transfersService = transfersService;
        _mapper = mapper;
    }

    [HttpGet(TransferRoutes.Get)]
    [Authorize(Roles = AppRoles.ViewPolicy)]
    [SwaggerOperation(
        Summary = "Get a transfer by Id",
        Description = "Get a transfer by Id",
        OperationId = "Transfers.GetById",
        Tags = new[] { "Transfers" })]
    public override async Task<ActionResult<TransferDto>> HandleAsync([FromRoute] ObjectIdRequest request,
        CancellationToken cancellationToken = new())
    {
        var transfer = await _transfersService.GetAsync(request.Id);
        if (transfer is null) return NotFound();

        var result = _mapper.Map<TransferDto>(transfer);

        return Ok(result);
    }
}
