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

public class
    CreateBudgetLevelEndpoint : EndpointBaseAsync.WithRequest<CreateBudgetLevelsRequest>.WithResult<BudgetLevelDto>
{
    private readonly IDbBaseRepository<BudgetLevel> _budgetLevelRepository;
    private readonly IValidator<CreateBudgetLevelsRequest> _validator;

    public CreateBudgetLevelEndpoint(IValidator<CreateBudgetLevelsRequest> validator, IDbBaseRepository<BudgetLevel> budgetLevelRepository)
    {
        _validator = validator;
        _budgetLevelRepository = budgetLevelRepository;
    }

    [Authorize(Roles = AppRoles.AdminPolicy)]
    [HttpPost("api/levels")]
    [SwaggerOperation(
        Summary = "Create budget level",
        Description = "Create budget level",
        OperationId = "BudgetLevels.Create",
        Tags = new[] { "BudgetLevels" })]
    public override async Task<BudgetLevelDto> HandleAsync(CreateBudgetLevelsRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var budgetLevel = new BudgetLevel
        {
            Level = request.Level,
            ShortTitle = request.ShortTitle,
            Title = request.Title
        };

        await _budgetLevelRepository.CreateAsync(budgetLevel);

        return BudgetLevelDto.FromBudgetLevel(budgetLevel);
    }
}
