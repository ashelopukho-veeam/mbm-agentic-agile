using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.CommonConfig;

public class GetConfig : EndpointBaseAsync.WithoutRequest.WithActionResult<CommonConfigDto>
{
    private readonly IApiService _apiService;
    private readonly IMapper _mapper;


    public GetConfig(IApiService apiService, IMapper mapper)
    {
        _apiService = apiService;
        _mapper = mapper;
    }

    [HttpGet("api/commonConfig")]
    [SwaggerOperation(
        Summary = "Get common app config",
        Description = "Get common app config",
        OperationId = "CommonConfig.Get",
        Tags = new[] { "CommonConfig" })]
    public override async Task<ActionResult<CommonConfigDto>> HandleAsync(
        CancellationToken cancellationToken = new())
    {
        var config = await _apiService.GetCommonConfigAsync();

        return _mapper.Map<CommonConfigDto>(config);
    }
}
