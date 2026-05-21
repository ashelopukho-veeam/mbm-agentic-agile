using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.UseCases.CommonConfig;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.CommonConfig;

[Authorize(Roles = AppRoles.AdminPolicy)]
public class SetConfig : EndpointBaseAsync.WithRequest<SetCommonConfigRequest>.WithResult<CommonConfigDto>
{
    private readonly IMapper _mapper;
    private readonly IValidator<SetCommonConfigRequest> _validator;
    private readonly IMediator _mediator;

    public SetConfig(IMapper mapper, IValidator<SetCommonConfigRequest> validator, IMediator mediator)
    {
        _mapper = mapper;
        _validator = validator;
        _mediator = mediator;
    }

    [HttpPost("api/commonConfig")]
    [SwaggerOperation(
        Summary = "Set common app config",
        Description = "Set common app config",
        OperationId = "CommonConfig.Set",
        Tags = new[] { "CommonConfig" })]
    public override async Task<CommonConfigDto> HandleAsync(SetCommonConfigRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var config = await _mediator.Send(new SetCommandConfigCommand { Request = request }, cancellationToken);

        return _mapper.Map<CommonConfigDto>(config);
    }
}
