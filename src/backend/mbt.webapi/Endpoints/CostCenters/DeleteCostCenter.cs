using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.CostCenters;

public class DeleteCostCenter : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithoutResult
{
    private readonly IDbBaseRepository<CostCenter> _costCenterRepository;
    private readonly IValidator<ObjectIdRequest> _validator;

    public DeleteCostCenter(IDbBaseRepository<CostCenter> costCenterRepository, IValidator<ObjectIdRequest> validator)
    {
        _costCenterRepository = costCenterRepository;
        _validator = validator;
    }

    [HttpDelete("api/costCenters/{Id}")]
    [Authorize(Roles = AppRoles.AdminPolicy)]
    [SwaggerOperation(
        Summary = "Delete cost center",
        Description = "Delete cost center",
        OperationId = "CostCenters.V2.Delete",
        Tags = new[] { "CostCenters" })]
    public override async Task HandleAsync(
        [FromRoute] ObjectIdRequest request, CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);

        var checkIfExist = await _costCenterRepository.GetAsync(request.Id);
        if (checkIfExist == null)
        {
            throw new ApiException("Cost center not found");
        }

        await _costCenterRepository.RemoveAsync(request.Id);
    }
}
