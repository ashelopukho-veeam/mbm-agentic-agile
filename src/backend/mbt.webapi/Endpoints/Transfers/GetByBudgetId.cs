using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.Transfers;

public class GetByBudgetId : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithResult<List<TransferExpanded>>
{
    private readonly ITransfersRepository _transfersRepository;
    private readonly IValidator<ObjectIdRequest> _validator;

    public GetByBudgetId(
        ITransfersRepository transfersRepository, IValidator<ObjectIdRequest> validator)
    {
        _transfersRepository = transfersRepository;
        _validator = validator;
    }

    [HttpGet("api/transfers/getByBudgetId/{Id}")]
    [Authorize(Roles = AppRoles.ViewPolicy)]
    [SwaggerOperation(
        Summary = "Get transfers by budget Id",
        Description = "Get transfers by budget Id",
        OperationId = "Transfers.GetByBudgetId",
        Tags = new[] { "Transfers" })]
    public override async Task<List<TransferExpanded>> HandleAsync([FromRoute] ObjectIdRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var transfers = await _transfersRepository.GetExpanded(
            t => t.FromBudgetId == request.Id || t.ToBudgetId == request.Id
        );

        return transfers;
    }
}
