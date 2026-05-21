using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using mbt.webapi.Jobs;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using IncrementalFundStatus = mbt.webapi.Domain.Entities.IncrementalFundStatus;
using MbtTaskStatus = mbt.webapi.Domain.Entities.MbtTaskStatus;

namespace mbt.webapi.WF.IncrementalFunds.Steps;

public class TaskResultStep : StepBody
{
    private readonly IApiService _apiService;
    private readonly IChatService _chatService;

    private readonly IMailService _mailService;
    private readonly ITaskService _taskService;
    private readonly IUserService _userService;

    private readonly IBudgetRepository _budgetRepository;

    private readonly IIncrementalFundsService _incrementalFundsService;

    public TaskResultStep(IMailService mailService, IUserService userService, ITaskService taskService,
        IApiService apiService, IChatService chatService, IIncrementalFundsService incrementalFundsService,
        IBudgetRepository budgetRepository)
    {
        _mailService = mailService;
        _userService = userService;
        _taskService = taskService;
        _apiService = apiService;
        _chatService = chatService;
        _incrementalFundsService = incrementalFundsService;
        _budgetRepository = budgetRepository;
    }

    [UsedImplicitly] public string TaskId { get; set; }

    public override ExecutionResult Run(IStepExecutionContext context)
    {
        var hostUri = _apiService.GetAppConfig().ClientHostUrl.Trim('/');
        var task = _taskService.Get(TaskId);
        var incrementalFund = _incrementalFundsService.Get(task.AssociatedItemId);
        var incrementalFundCreatedByUserEmail = _userService.Get(incrementalFund.CreatedBy).Email;

        var toBudget = _budgetRepository.Get(incrementalFund.ToBudgetId);

        var taskModifiedBy = _userService.Get(task.ModifiedBy).DisplayName;
        string chatMsg;

        if (task.Outcome == MbtTaskStatus.Rejected)
        {
            incrementalFund.Status = IncrementalFundStatus.Rejected;
            _incrementalFundsService.Update(incrementalFund);

            chatMsg = string.Format(ChatSystemMessages.ItemRejected, "Incremental Fund", taskModifiedBy);
            if (task.ModifiedBy != task.AssignedTo)
                chatMsg += $" on behalf of {_userService.Get(task.AssignedTo).DisplayName}";
            if (!string.IsNullOrWhiteSpace(task.Comment)) chatMsg += $" with a comment: '{task.Comment}'";

            _chatService.AddSystemChatMessage(task.AssociatedItemId, chatMsg);


            _mailService.QueueAsync(incrementalFundCreatedByUserEmail,
                GroupedMailTemplates.IncrementalFunds.Rejected.Subject,
                MailTemplates.GetIncrementalFund_Rejected_MailBody(
                    incrementalFund.Title,
                    toBudget.Title,
                    incrementalFund.Amount,
                    task.Comment));
        }
        else if (task.Outcome == MbtTaskStatus.SendBackToEdit)
        {
            incrementalFund.Status = IncrementalFundStatus.Draft;
            _incrementalFundsService.Update(incrementalFund);

            chatMsg = string.Format(ChatSystemMessages.ItemRejectedToInitiator, "Incremental Fund", taskModifiedBy);
            if (!string.IsNullOrWhiteSpace(task.Comment)) chatMsg += $" with a comment: '{task.Comment}'";
            if (task.ModifiedBy != task.AssignedTo)
                chatMsg += $" on behalf of {_userService.Get(task.AssignedTo).DisplayName}";
            _chatService.AddSystemChatMessage(incrementalFund.Id, chatMsg);

            var incrementalFundEditFormUrl = $"{hostUri}/incremental-funds/edit/{incrementalFund.Id}";
            _mailService.QueueAsync(incrementalFundCreatedByUserEmail,
                GroupedMailTemplates.IncrementalFunds.SentBackToEdit.Subject,
                MailTemplates.GetIncrementalFund_SendBack_MailBody(
                    incrementalFund.Title,
                    toBudget.Title,
                    incrementalFund.Amount,
                    task.Comment,
                    incrementalFundEditFormUrl));
        }
        else if (task.Outcome == MbtTaskStatus.Approved)
        {
            var msg = string.Format(ChatSystemMessages.ItemApproved, "Incremental Fund", taskModifiedBy);
            if (task.ModifiedBy != task.AssignedTo)
                msg += $" on behalf of {_userService.Get(task.AssignedTo).DisplayName}";
            _chatService.AddSystemChatMessage(incrementalFund.Id,
                msg);
        }

        return ExecutionResult.Next();
    }
}
