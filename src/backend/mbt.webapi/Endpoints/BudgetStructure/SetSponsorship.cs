using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.BuiltIn;
using mbt.webapi.Endpoints.BudgetStructure.dto;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using QuartersValues = mbt.webapi.Domain.Entities.QuartersValues;

namespace mbt.webapi.Endpoints.BudgetStructure;

public class SetSponsorship : EndpointBaseAsync.WithRequest<SetSponsorshipRequest>.WithoutResult
{
    private readonly IBudgetRepository _budgetRepository;

    public SetSponsorship(IBudgetRepository budgetRepository)
    {
        _budgetRepository = budgetRepository;
    }

    [HttpPost("api/budgetStructure/sponsorship")]
    [Authorize(Roles = AppRoles.AdminPolicy)]
    [SwaggerOperation(
        Summary = "Set budget structure sponsorship values",
        Description = "Set budget structure sponsorship values",
        OperationId = "BudgetStructure.SetSponsorship",
        Tags = new[] { "BudgetStructure" })]
    public override async Task<ActionResult> HandleAsync(SetSponsorshipRequest request,
        CancellationToken cancellationToken = new())
    {
        var budget = await _budgetRepository.GetAsync(request.Id);
        if (budget == null)
            return NotFound();

        var values = new QuartersValues()
        {
            Q1 = request.Q1,
            Q2 = request.Q2,
            Q3 = request.Q3,
            Q4 = request.Q4
        };
        budget.Sponsorship = values;

        await _budgetRepository.UpdateAsync(budget);

        return Ok();
    }
}
