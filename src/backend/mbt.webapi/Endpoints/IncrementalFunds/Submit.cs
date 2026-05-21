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
using mbt.webapi.WF.IncrementalFunds.v2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WorkflowCore.Interface;
using IncrementalFundStatus = mbt.webapi.Domain.Entities.IncrementalFundStatus;

namespace mbt.webapi.Endpoints.IncrementalFunds;

public class Submit : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithoutResult
{
    private readonly IIncrementalFundsService _incrementalFundsService;
    private readonly IWorkflowController _workflowService;
    private readonly IChatService _chatService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IValidator<ObjectIdRequest> _validator;
    private readonly IDbBaseRepository<PaidMediaTeamApprover> _paidMediaTeamApproverRepository;

    public Submit(IIncrementalFundsService incrementalFundsService, IWorkflowController workflowService,
        IChatService chatService, ICurrentUserContext currentUserContext,
        IDbBaseRepository<PaidMediaTeamApprover> paidMediaTeamApproverRepository, IValidator<ObjectIdRequest> validator)
    {
        _incrementalFundsService = incrementalFundsService;
        _workflowService = workflowService;
        _chatService = chatService;
        _currentUserContext = currentUserContext;
        _paidMediaTeamApproverRepository = paidMediaTeamApproverRepository;
        _validator = validator;
    }


    [HttpPost(IncrementalFundsRoutes.Submit)]
    [Authorize(Roles = $"{AppRoles.AdminPolicy},{AppRoles.Designers},{AppRoles.Contributors}")]
    [SwaggerOperation(
        Summary = "Submit an incremental fund",
        Description = "Submit an incremental fund",
        OperationId = "IncrementalFunds.Submit",
        Tags = new[] { "IncrementalFunds" })]
    public override async Task<ActionResult> HandleAsync([FromRoute] ObjectIdRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var incrementalFundExpanded = await _incrementalFundsService.GetExpanded(request.Id);
        if (incrementalFundExpanded == null) return NotFound();

        // validate Admin or Author
        var isAdmin = _currentUserContext.IsInRoles(new[] { AppRoles.SysAdmins });
        var isAuthor = _currentUserContext.UserId == incrementalFundExpanded.CreatedBy;

        if (!isAdmin && !isAuthor)
            throw new AccessDeniedException();

        if (incrementalFundExpanded.Status != IncrementalFundStatus.Draft)
            throw new ApiException("An incremental fund does not have the 'Draft' status.");

        var hasPaidMediaBudget = incrementalFundExpanded.ToBudget.IsPaidMedia;

        PaidMediaTeamApprover paidMediaTeamApprover = null;
        if (hasPaidMediaBudget)
        {
            paidMediaTeamApprover =
                await _paidMediaTeamApproverRepository.FindOneAsync(p => p.Team == incrementalFundExpanded.Team);
            if (paidMediaTeamApprover == null)
                throw new ApiException(ErrorMessages.PaidMediaTeamApproverNotSet(incrementalFundExpanded.Team));
        }

        var config = await _incrementalFundsService.GetConfig();

        if (config == null)
            throw new ApiException("Incremental funds config is not set.");

        var globalApproverId = config.WorkflowApproverId;

        var workflowData = new IncrementalFundsApproveWorkflowDataV2()
        {
            GlobalApproverId = globalApproverId,
            BudgetOwnerId = incrementalFundExpanded.ToBudget.OwnerId,
            IncrementalFundItem = incrementalFundExpanded,
            PaidMediaTeamApproverId = paidMediaTeamApprover?.ApproverId
        };


        var wfId = await _workflowService.StartWorkflow(WorkflowNames.IncrementalFundsApproveWorkflow, 2, workflowData);

        var incrementalFund = await _incrementalFundsService.GetAsync(request.Id);
        incrementalFund.Status = IncrementalFundStatus.PendingApproval;
        incrementalFund.WorkflowId = wfId;
        await _incrementalFundsService.UpdateAsync(incrementalFund);

        await _chatService.AddSystemChatMessageAsync(incrementalFundExpanded.Id,
            $"Incremental Fund is submitted by {_currentUserContext.UserName}");

        return Ok();
    }
}
