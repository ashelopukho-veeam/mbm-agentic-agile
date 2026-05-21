using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.Admin;

[Authorize(Roles = AppRoles.AdminPolicy)]
public class
    GetPaidMediaTeamApprovers : EndpointBaseAsync.WithoutRequest.WithResult<List<PaidMediaTeamApproverExpanded>>
{
    private readonly IPaidMediaTeamApproverRepository _paidMediaTeamApproverRepository;

    public GetPaidMediaTeamApprovers(IPaidMediaTeamApproverRepository paidMediaTeamApproverRepository)
    {
        _paidMediaTeamApproverRepository = paidMediaTeamApproverRepository;
    }

    [HttpGet(PaidMediaTeamApproverRoutes.List)]
    [SwaggerOperation(
        Summary = "Paid media team approvers",
        Description = "Get paid media team approvers",
        OperationId = "Admin.GetPaidMediaTeamApprovers",
        Tags = new[] { "Admin" })]
    public override async Task<List<PaidMediaTeamApproverExpanded>> HandleAsync(
        CancellationToken cancellationToken = new())
    {
        var approvers = await _paidMediaTeamApproverRepository
            .GetExpanded();

        return approvers;
    }
}
