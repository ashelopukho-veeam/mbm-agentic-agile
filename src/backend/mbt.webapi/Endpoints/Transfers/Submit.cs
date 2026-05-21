using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using mbt.webapi.BuiltIn;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Extensions;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using mbt.webapi.WF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WorkflowCore.Interface;

namespace mbt.webapi.Endpoints.Transfers;

public class Submit : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithoutResult
{
    private readonly ITransfersService _transfersService;
    private readonly IWorkflowController _workflowService;
    private readonly IChatService _chatService;
    private readonly IApiService _apiService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IValidator<ObjectIdRequest> _validator;
    private readonly IDbBaseRepository<PaidMediaTeamApprover> _paidMediaTeamApproverRepository;

    public Submit(ITransfersService transfersService, IWorkflowController workflowService,
        IChatService chatService, IApiService apiService, ICurrentUserContext currentUserContext,
        IValidator<ObjectIdRequest> validator, IDbBaseRepository<PaidMediaTeamApprover> paidMediaTeamApproverRepository)
    {
        _transfersService = transfersService;
        _workflowService = workflowService;
        _chatService = chatService;
        _apiService = apiService;
        _currentUserContext = currentUserContext;
        _validator = validator;
        _paidMediaTeamApproverRepository = paidMediaTeamApproverRepository;
    }


    [HttpPost("api/transfers/{Id}/submit")]
    [Authorize(Roles = $"{AppRoles.AdminPolicy},{AppRoles.Designers},{AppRoles.Contributors}")]
    [SwaggerOperation(
        Summary = "Submit a transfer",
        Description = "Submit a transfer",
        OperationId = "Transfers.Submit",
        Tags = new[] { "Transfers" })]
    public override async Task<ActionResult> HandleAsync([FromRoute] ObjectIdRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var transferExpanded = await _transfersService.GetExpanded(request.Id);

        if (transferExpanded == null) return NotFound();

        // validate Admin or Author
        var isAdmin = _currentUserContext.IsInRoles(new[] { AppRoles.SysAdmins });
        var isAuthor = _currentUserContext.UserId == transferExpanded.CreatedBy;

        if (!isAdmin && !isAuthor)
            throw new AccessDeniedException();

        if (transferExpanded.Status != TransferStatus.Draft) throw new Exception("Transfer is not in draft status");

        var hasPaidMediaBudget = transferExpanded.FromBudget.IsPaidMedia || transferExpanded.ToBudget.IsPaidMedia;

        PaidMediaTeamApprover paidMediaTeamApprover = null;
        if (hasPaidMediaBudget)
        {
            paidMediaTeamApprover = await _paidMediaTeamApproverRepository.FindOneAsync(p => p.Team == transferExpanded.Team);
            if (paidMediaTeamApprover == null)
                throw new ApiException( ErrorMessages.PaidMediaTeamApproverNotSet(transferExpanded.Team));
        }

        var appConfig = await _apiService.GetAppConfigAsync();
        var wfData = new SubmitTransferWorkflowDataV2
        {
            HostUri = appConfig.ClientHostUrl.Trim('/'),
            TransferId = transferExpanded.Id,
            Transfer = transferExpanded,
            RequestorEmail = transferExpanded.CreatedByUser.Email,
            PaidMediaTeamApproverId = paidMediaTeamApprover?.ApproverId
        };

        var wfId = await _workflowService.StartWorkflow(WorkflowNames.SubmitTransferWorkflow, wfData);

        var t = await _transfersService.GetAsync(transferExpanded.Id);
        t.Status = TransferStatus.PendingApproval;
        t.WorkflowId = wfId;
        _transfersService.Update(t);

        await _chatService.AddSystemChatMessageAsync(t.Id, ChatSystemMessages.TransferSubmited + _currentUserContext.UserName);

        return Ok();
    }
}
