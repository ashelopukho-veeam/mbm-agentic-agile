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

public class ExpireTransfer : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithResult<TransferDto>
{
    private readonly ITransfersService _transfersService;
    private readonly IMapper _mapper;

    public ExpireTransfer(ITransfersService transfersService, IMapper mapper)
    {
        _transfersService = transfersService;
        _mapper = mapper;
    }

    [Authorize(Roles = AppRoles.AdminPolicy)]
    [HttpPost("api/transfers/{Id}/expire")]
    [SwaggerOperation(
        Summary = "Expire transfer",
        Description = "Expire transfer",
        OperationId = "Transfers.Expire",
        Tags = new[] { "Transfers" })]
    public override async Task<TransferDto> HandleAsync([FromRoute] ObjectIdRequest request,
        CancellationToken cancellationToken = new())
    {
        var transfer = await _transfersService.ExpireTransfer(request.Id);

        return _mapper.Map<TransferDto>(transfer);
    }
}
