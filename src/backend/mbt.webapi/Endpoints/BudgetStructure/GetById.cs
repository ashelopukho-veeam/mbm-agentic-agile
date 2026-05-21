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

namespace mbt.webapi.Endpoints.BudgetStructure;

public class GetById : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithActionResult<BudgetStructureExpanded>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IValidator<ObjectIdRequest> _validator;

    public GetById(IBudgetRepository budgetRepository, IValidator<ObjectIdRequest> validator)
    {
        _budgetRepository = budgetRepository;
        _validator = validator;
    }

    [HttpGet("api/budgetStructure/{Id}")]
    [Authorize(Roles = AppRoles.ViewPolicy)]
    [SwaggerOperation(
        Summary = "Get a Budget Structure by Id",
        Description = "Get a Budget Structure by Id",
        OperationId = "BudgetStructure.GetById",
        Tags = new[] { "BudgetStructure" })]
    public override async Task<ActionResult<BudgetStructureExpanded>> HandleAsync([FromRoute] ObjectIdRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var budget = await _budgetRepository.GetByIdExpanded(request.Id);
        if (budget == null)
            return NotFound();

        return budget;
    }
}
