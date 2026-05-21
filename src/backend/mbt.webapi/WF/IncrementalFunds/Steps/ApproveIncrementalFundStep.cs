using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using mbt.webapi.Jobs;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using IncrementalFundStatus = mbt.webapi.Domain.Entities.IncrementalFundStatus;

namespace mbt.webapi.WF.IncrementalFunds.Steps;

[PublicAPI]
public class ApproveIncrementalFundStep : StepBody
{
    private readonly IChatService _chatService;
    private readonly IIncrementalFundsService _incrementalFundsService;
    private readonly IMailService _mailService;
    private readonly IUserService _userService;
    private readonly IBudgetRepository _budgetRepository;

    public string IncrementalFundId { get; set; }


    public ApproveIncrementalFundStep(IUserService userService,
        IChatService chatService,
        IIncrementalFundsService incrementalFundsService,
        IMailService mailService,
        IBudgetRepository budgetRepository)
    {
        _userService = userService;
        _incrementalFundsService = incrementalFundsService;
        _chatService = chatService;
        _mailService = mailService;
        _budgetRepository = budgetRepository;
    }


    public override ExecutionResult Run(IStepExecutionContext context)
    {
        var incrementalFund = _incrementalFundsService.Get(IncrementalFundId);
        incrementalFund.Status = IncrementalFundStatus.Approved;
        _incrementalFundsService.Update(incrementalFund);
        _chatService.AddSystemChatMessage(IncrementalFundId, "Incremental Fund is approved");

        var requester = _userService.Get(incrementalFund.CreatedBy);

        var toBudget = _budgetRepository.Get(incrementalFund.ToBudgetId);

        _mailService.QueueAsync(requester.Email,
            GroupedMailTemplates.IncrementalFunds.Approved.Subject,
            MailTemplates.GetIncrementalFund_Approved_MailBody(incrementalFund.Title, toBudget.Title,
                incrementalFund.Amount));

        return ExecutionResult.Next();
    }
}
