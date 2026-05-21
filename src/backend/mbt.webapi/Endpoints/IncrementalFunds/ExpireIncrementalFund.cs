using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Endpoints.IncrementalFunds.dto;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.IncrementalFunds;

public class ExpireIncrementalFund : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithResult<IncrementalFundDto>
{
    private readonly IIncrementalFundsService _incrementalFundsService;
    private readonly IMapper _mapper;
    private readonly IValidator<ObjectIdRequest> _validator;

    public ExpireIncrementalFund(
        IIncrementalFundsService incrementalFundsService,
        IMapper mapper,
        IValidator<ObjectIdRequest> validator)
    {
        _incrementalFundsService = incrementalFundsService;
        _mapper = mapper;
        _validator = validator;
    }

    [Authorize(Roles = AppRoles.AdminPolicy)]
    [HttpPost("api/incrementalFunds/{Id}/expire")]
    [SwaggerOperation(
        Summary = "Expire incremental fund",
        Description = "Expire incremental fund",
        OperationId = "IncrementalFunds.Expire",
        Tags = new[] { "IncrementalFunds" })]
    public override async Task<IncrementalFundDto> HandleAsync([FromRoute] ObjectIdRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);
        var incrementalFund = await _incrementalFundsService.ExpireIncrementalFund(request.Id);

        return _mapper.Map<IncrementalFundDto>(incrementalFund);
    }
}
