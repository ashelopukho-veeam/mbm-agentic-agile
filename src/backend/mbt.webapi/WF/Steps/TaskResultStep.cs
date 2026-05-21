using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Jobs;
using mbt.webapi.Services;
using mbt.webapi.Services.Interfaces;
using mbt.webapi.Utils;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace mbt.webapi.WF.Steps;

[UsedImplicitly]
public class TaskResultStep : StepBodyAsync
{
    private readonly IApiService _apiService;
    private readonly IChatService _chatService;

    private readonly IMailService _mailService;
    private readonly ITaskService _taskService;
    private readonly ITransfersService _transfersService;
    private readonly IUserService _userService;

    public TaskResultStep(IMailService mailService, IUserService userService, ITaskService taskService,
        IApiService apiService, ITransfersService transfersService, IChatService chatService)
    {
        _mailService = mailService;
        _userService = userService;
        _taskService = taskService;
        _apiService = apiService;
        _transfersService = transfersService;
        _chatService = chatService;
    }

    public string TaskId { get; set; }
    public string TransferId { get; set; }

    public override async Task<ExecutionResult> RunAsync(IStepExecutionContext context)
    {
        var hostUri = (await _apiService.GetAppConfigAsync()).ClientHostUrl.Trim('/');
        var task = await _taskService.GetAsync(TaskId);
        var transferId = task.AssociatedItemId;
        var transferExpanded = await _transfersService.GetExpanded(transferId);
        var transfer = await _transfersService.GetAsync(transferId);


        var assignedTo = await _userService.GetAsync(task.AssignedTo);
        var taskModifiedBy = (await _userService.GetAsync(task.ModifiedBy)).DisplayName;
        string chatMsg;

        if (task.Outcome == MbtTaskStatus.Rejected)
        {
            transfer.Status = TransferStatus.Rejected;
            _transfersService.Update(transfer);

            chatMsg = ChatSystemMessages.TransferRejected + taskModifiedBy;
            if (!string.IsNullOrWhiteSpace(task.Comment)) chatMsg += $" with a comment: '{task.Comment}'";

            await _chatService.AddSystemChatMessageAsync(transferId, chatMsg);

            await _mailService.QueueAsync(transferExpanded.CreatedByUser.Email,
                GroupedMailTemplates.Transfers.Rejected.Subject,
                MailTemplates.GetTransferRejectedEmail(transfer.Title,
                    transferExpanded.FromBudget.Title, transferExpanded.ToBudget.Title,
                    transferExpanded.Amount.ToString(CultureInfo.InvariantCulture), task.Comment));
        }
        else if (task.Outcome == MbtTaskStatus.SendBackToEdit)
        {
            transfer.Status = TransferStatus.Draft;
            _transfersService.Update(transfer);

            chatMsg = ChatSystemMessages.TransferRejectedToInitiator + taskModifiedBy;
            if (!string.IsNullOrWhiteSpace(task.Comment)) chatMsg += $" with a comment: '{task.Comment}'";

            await _chatService.AddSystemChatMessageAsync(transferId, chatMsg);

            var transferEditPageUrl = UrlUtils.Combine(hostUri, SpaRoutes.TransfersEditItem(transfer.Id));
            await _mailService.QueueAsync(transferExpanded.CreatedByUser.Email,
                GroupedMailTemplates.Transfers.SentBackToEdit.Subject,
                MailTemplates.GetTransferRejectedToInitiatorEmail(
                    transfer.Title,
                    transferExpanded.FromBudget.Title, transferExpanded.ToBudget.Title,
                    transferExpanded.Amount.ToString(CultureInfo.InvariantCulture), task.Comment,
                    transferEditPageUrl));
        }
        else if (task.Outcome == MbtTaskStatus.Approved)
        {
            var msg = "Transfer is approved by: " + taskModifiedBy;
            if (task.ModifiedBy != task.AssignedTo)
            {
                msg += $" on behalf of {assignedTo.DisplayName}";
            }

            await _chatService.AddSystemChatMessageAsync(transferId,
                msg);
        }


        return ExecutionResult.Next();
    }
}
