using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.IncrementalFunds;

public class
    GetByBudgetId : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithResult<List<IncrementalFundExpanded>>
{
    private readonly IMapper _mapper;
    private readonly IIncrementalFundsRepository _incrementalFundRepository;
    private readonly IValidator<ObjectIdRequest> _validator;

    public GetByBudgetId(IMapper mapper,
        IIncrementalFundsRepository incrementalFundRepository, IValidator<ObjectIdRequest> validator)
    {
        _mapper = mapper;
        _incrementalFundRepository = incrementalFundRepository;
        _validator = validator;
    }

    [HttpGet("api/incrementalFunds/getByBudgetId/{Id}")]
    [Authorize(Roles = AppRoles.ViewPolicy)]
    [SwaggerOperation(
        Summary = "Get incremental funds by budget Id",
        Description = "Get incremental funds by budget Id",
        OperationId = "IncrementalFunds.GetByBudgetId",
        Tags = new[] { "IncrementalFunds" })]
    public override async Task<List<IncrementalFundExpanded>> HandleAsync([FromRoute] ObjectIdRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);

        var filter = new ExpressionFilterDefinition<IncrementalFundExpanded>(inc => inc.ToBudgetId == request.Id);
        var incrementalFunds = await _incrementalFundRepository.GetExpanded(filter);

        return incrementalFunds;
    }
}
