using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Endpoints.BudgetStructure.dto;
using mbt.webapi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.BudgetStructure;

public class ListItems : EndpointBaseAsync
    .WithRequest<ListItemsRequest>
    .WithResult<List<BudgetStructureExpanded>>
{
    private readonly IBudgetRepository _budgetRepository;

    public ListItems(IBudgetRepository budgetRepository)
    {
        _budgetRepository = budgetRepository;
    }

    [HttpGet("api/budgetStructure")]
    [Authorize(Roles = AppRoles.ViewPolicy)]
    [SwaggerOperation(
        Summary = "List budget structures",
        Description = "List budget structures",
        OperationId = "BudgetStructure.ListItems",
        Tags = new[] { "BudgetStructure" })]
    public override async Task<List<BudgetStructureExpanded>>
        HandleAsync([FromQuery] ListItemsRequest request,
            CancellationToken cancellationToken = new())
    {
        var filter = new ExpressionFilterDefinition<BudgetStructureExpanded>(b => b.Year == request.Year);
        var budgets = await _budgetRepository.GetExpanded(filter);

        return budgets;
    }
}
