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

namespace mbt.webapi.Endpoints.Admin;

[Authorize(Roles = AppRoles.AdminPolicy)]
public class SetPaidMediaTeamApprover : EndpointBaseAsync.WithRequest<SetPaidMediaTeamApproverRequest>.WithResult<
    PaidMediaTeamApproverExpanded>
{
    private readonly IPaidMediaTeamApproverRepository _paidMediaTeamApproverRepository;
    private readonly IValidator<SetPaidMediaTeamApproverRequest> _validator;

    public SetPaidMediaTeamApprover(
        IValidator<SetPaidMediaTeamApproverRequest> validator,
        IPaidMediaTeamApproverRepository paidMediaTeamApproverRepository)
    {
        _validator = validator;
        _paidMediaTeamApproverRepository = paidMediaTeamApproverRepository;
    }


    [HttpPost(PaidMediaTeamApproverRoutes.Set)]
    [SwaggerOperation(
        Summary = "Paid media team approvers",
        Description = "Set paid media team approvers",
        OperationId = "Admin.SetPaidMediaTeamApprovers",
        Tags = new[] { "Admin" })]
    public override async Task<PaidMediaTeamApproverExpanded> HandleAsync(
        [FromBody] SetPaidMediaTeamApproverRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var config = new PaidMediaTeamApprover()
        {
            Team = request.Team,
            ApproverId = request.ApproverId.ToString()
        };

        var configItem = await _paidMediaTeamApproverRepository.FindOneAsync(r => r.Team == request.Team);
        if (configItem != null)
        {
            config.Id = configItem.Id;
            await _paidMediaTeamApproverRepository.UpdateAsync(config);
        }
        else
        {
            await _paidMediaTeamApproverRepository.CreateAsync(config);
        }

        var configItemExpandedResult = await _paidMediaTeamApproverRepository
            .GetByIdExpanded(config.Id);


        return configItemExpandedResult;
    }
}
