using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.BudgetLevels;

public class DeleteBudgetLevelEndpoint : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithResult<bool>
{
    private readonly IDbBaseRepository<BudgetLevel> _budgetLevelRepository;
    private readonly IValidator<ObjectIdRequest> _validator;

    public DeleteBudgetLevelEndpoint(IDbBaseRepository<BudgetLevel> budgetLevelRepository,
        IValidator<ObjectIdRequest> validator)
    {
        _budgetLevelRepository = budgetLevelRepository;
        _validator = validator;
    }

    [Authorize(Roles = AppRoles.AdminPolicy)]
    [HttpDelete("api/levels/{Id}")]
    [SwaggerOperation(
        Summary = "Delete budget level",
        Description = "Delete budget level",
        OperationId = "BudgetLevels.Delete",
        Tags = new[] { "BudgetLevels" })]
    public override async Task<bool> HandleAsync([FromRoute] ObjectIdRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        await _budgetLevelRepository.RemoveAsync(request.Id);

        return true;
    }
}
