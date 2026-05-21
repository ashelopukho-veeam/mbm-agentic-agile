using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Jobs;
using mbt.webapi.Services.Interfaces;

namespace mbt.webapi.Services;

[UsedImplicitly]
public class BudgetPlanService : IBudgetPlanService
{
    private readonly IBudgetService _budgetService;
    private readonly IUserService _userService;
    private readonly IMailService _mailService;
    private readonly IChatService _chatService;
    private readonly ICurrentUserContext _currentUserContext;

    public BudgetPlanService(IBudgetService budgetService, IUserService userService,
        IMailService mailService, IChatService chatService, ICurrentUserContext currentUserContext)
    {
        _budgetService = budgetService;
        _userService = userService;
        _mailService = mailService;
        _chatService = chatService;
        _currentUserContext = currentUserContext;
    }

    private async Task SetPlanStatus(string planId, string status)
    {
        var budget = await _budgetService.GetBudgetByPlanId(planId);
        var plan = budget.GetBudgetPlanById(planId);
        plan.Status = status;
        await _budgetService.UpdateAsync(budget);
    }


    private Task AddChatMessages(string chatParentItemId, string action, string comment = "")
    {
        var msg = GetBudgetPlanStatusSystemMessage(action);
        if (!string.IsNullOrWhiteSpace(comment)) msg += $" with comment '{comment}'";

        return _chatService.AddSystemChatMessageAsync(chatParentItemId, msg);
    }

    private string GetBudgetPlanStatusSystemMessage(string status)
    {
        string msg;
        switch (status)
        {
            case BudgetPlanStatusAction.ReturnToDraft:
                msg = ChatSystemMessages.BudgetPlanReturnedToDraft;
                break;
            case BudgetPlanStatusAction.Reject:
                msg = ChatSystemMessages.BudgetPlanRejected;
                break;
            case BudgetPlanStatusAction.Approve:
                msg = ChatSystemMessages.BudgetPlanApproved;
                break;
            case BudgetPlanStatusAction.SubmitToFinalApproval:
                msg = ChatSystemMessages.BudgetPlanSubmittedToFinalApprove;
                break;
            case BudgetPlanStatusAction.SubmitToOwner:
                msg = ChatSystemMessages.BudgetPlanSubmittedToOwner;
                break;
            default:
                return "Message not found for status: " + status;
        }

        return msg.Replace(ChatSystemMessages.UserTag, _currentUserContext.UserName);
    }


    public async Task DraftToSubmitToOwnerStep(string budgetPlanId, string comment, bool notify)
    {
        var budget = await _budgetService.GetBudgetByPlanId(budgetPlanId);
        var plan = budget.GetBudgetPlanById(budgetPlanId);
        var isOriginalForecast = plan.Quarter == Quarters.Q1;

        // validate permissions
        var isValid = _currentUserContext.IsInRoles(new[] { AppRoles.SysAdmins, AppRoles.Admins }) ||
                      (_currentUserContext.IsInRole(AppRoles.Contributors) &&
                       (_currentUserContext.UserId == budget.ParentManagerId ||
                        _currentUserContext.UserId == budget.OwnerId));
        if (!isValid)
            throw new AccessDeniedException();

        // set status
        await SetPlanStatus(budgetPlanId, BudgetPlanStatus.SubmittedToOwner);

        // send notifications
        await AddChatMessages(budget.Id, BudgetPlanStatusAction.SubmitToOwner, comment);

        var subject = isOriginalForecast
            ? GroupedMailTemplates.BudgetPlanTemplates.OriginalForecast.SubmitToOwner.Subject
            : GroupedMailTemplates.BudgetPlanTemplates.Reforecast.SubmitToOwner.Subject;
        var owner = await _userService.GetAsync(budget.OwnerId);
        var body = budget.Title;
        await _mailService.QueueAsync(owner.Email, subject, body);
    }

    public async Task SubmittedToOwnerToPendingApprovalInOriginalForecastStep(string budgetPlanId, string comment,
        bool notify)
    {
        var budget = await _budgetService.GetBudgetByPlanId(budgetPlanId);

        // validate permissions
        var isValid = _currentUserContext.IsInRoles(new[] { AppRoles.SysAdmins }) ||
                      (_currentUserContext.IsInRole(AppRoles.Contributors) &&
                       _currentUserContext.UserId == budget.OwnerId);
        if (!isValid)
            throw new AccessDeniedException();

        // set status
        await SetPlanStatus(budgetPlanId, BudgetPlanStatus.PendingApproval);

        // send notifications
        await AddChatMessages(budget.Id, BudgetPlanStatusAction.SubmitToFinalApproval, comment);
    }

    public async Task SubmittedToOwnerToDraftInOriginalForecastStep(string budgetPlanId, string comment, bool notify)
    {
        var budget = await _budgetService.GetBudgetByPlanId(budgetPlanId);

        // validate permissions
        var isValid = _currentUserContext.IsInRoles(new[] { AppRoles.SysAdmins }) ||
                      (_currentUserContext.IsInRole(AppRoles.Contributors) &&
                       _currentUserContext.UserId == budget.OwnerId);
        if (!isValid)
            throw new AccessDeniedException();
        // set status
        await SetPlanStatus(budgetPlanId, BudgetPlanStatus.Draft);

        // send notifications
        var parentManager = await _userService.GetAsync(budget.ParentManagerId);
        var listTo = new List<string> { parentManager.Email };
        if (notify)
        {
            var manager = await _userService.GetAsync(budget.ManagerId);
            listTo.Add(manager.Email);
        }

        var body = MailTemplates.BudgetPlanRejected(budget.Title, comment);
        await _mailService.QueueAsync(listTo,
            GroupedMailTemplates.BudgetPlanTemplates.OriginalForecast.Rejected.Subject,
            body);

        await AddChatMessages(budget.Id, BudgetPlanStatusAction.ReturnToDraft, comment);
    }

    public async Task PendingApprovalToDraftInOriginalForecastStep(string budgetPlanId, string comment, bool notify)
    {
        var budget = await _budgetService.GetBudgetByPlanId(budgetPlanId);

        // validate permissions
        var isValid = _currentUserContext.IsInRoles(new[]
            { AppRoles.SysAdmins, AppRoles.Admins, AppRoles.GlobalApprovers });
        if (!isValid)
            throw new AccessDeniedException();

        // set status
        await SetPlanStatus(budgetPlanId, BudgetPlanStatus.Draft);

        // send notifications
        await AddChatMessages(budget.Id, BudgetPlanStatusAction.Reject, comment);
        var owner = await _userService.GetAsync(budget.OwnerId);
        var listTo = new List<string> { owner.Email };
        if (notify)
        {
            var parentManager = await _userService.GetAsync(budget.ParentManagerId);
            var manager = await _userService.GetAsync(budget.ManagerId);
            listTo.Add(parentManager.Email);
            listTo.Add(manager.Email);
        }

        var body = MailTemplates.BudgetPlanRejected(budget.Title, comment);
        await _mailService.QueueAsync(listTo,
            GroupedMailTemplates.BudgetPlanTemplates.OriginalForecast.Rejected.Subject,
            body);
    }

    public async Task PendingApprovalToApprovedInOriginalForecastStep(string budgetPlanId, string comment, bool notify)
    {
        var budget = await _budgetService.GetBudgetByPlanId(budgetPlanId);
        var plan = budget.GetBudgetPlanById(budgetPlanId);
        // validate permissions
        var isValid = _currentUserContext.IsInRoles(new[]
            { AppRoles.SysAdmins, AppRoles.Admins, AppRoles.GlobalApprovers });
        if (!isValid)
            throw new AccessDeniedException();

        // set status
        plan.Status = BudgetPlanStatus.Approved;

        // copy forecast data to the next forecast
        var nextPlanIndex = budget.Plans.IndexOf(plan) + 1;
        if (nextPlanIndex < budget.Plans.Count)
        {
            var nextPlan = budget.Plans[nextPlanIndex];
            CopyForecastData(plan, nextPlan);
        }

        await _budgetService.UpdateAsync(budget);


        // send notifications
        await AddChatMessages(budget.Id, BudgetPlanStatusAction.Approve, comment);
        var owner = await _userService.GetAsync(budget.OwnerId);
        var parentManager = await _userService.GetAsync(budget.ParentManagerId);
        var manager = await _userService.GetAsync(budget.ManagerId);
        await _mailService.QueueAsync(new List<string> { owner.Email, parentManager.Email, manager.Email },
            GroupedMailTemplates.BudgetPlanTemplates.OriginalForecast.Approved.Subject,
            budget.Title);
    }

    private void CopyForecastData(BudgetPlan plan, BudgetPlan nextPlan)
    {
        nextPlan.Q1 = plan.Q1;
        nextPlan.Q2 = plan.Q2;
        nextPlan.Q3 = plan.Q3;
        nextPlan.Q4 = plan.Q4;
        nextPlan.Segments = new List<TitleNumberValuePair>();
        nextPlan.Segments.AddRange(plan.Segments);
        nextPlan.Campaigns = new List<TitleNumberValuePair>();
        nextPlan.Campaigns.AddRange(plan.Campaigns);
    }

    public async Task SubmittedToOwnerToApprovedInReforecastStep(string budgetPlanId, string comment, bool notify)
    {
        var budget = await _budgetService.GetBudgetByPlanId(budgetPlanId);

        var isValid = _currentUserContext.IsInRoles(new[]
                          { AppRoles.SysAdmins, AppRoles.Admins }) ||
                      (_currentUserContext.IsInRole(AppRoles.Contributors) &&
                       _currentUserContext.UserId == budget.OwnerId);

        if (!isValid)
            throw new AccessDeniedException();

        await SetPlanStatus(budgetPlanId, BudgetPlanStatus.Approved);

        await AddChatMessages(budget.Id, BudgetPlanStatusAction.Approve, comment);
        var manager = await _userService.GetAsync(budget.ManagerId);
        var parentManager = await _userService.GetAsync(budget.ParentManagerId);
        await _mailService.QueueAsync(new List<string> { manager.Email, parentManager.Email },
            GroupedMailTemplates.BudgetPlanTemplates.Reforecast.Approved.Subject,
            budget.Title);
    }

    public async Task SubmittedToOwnerToDraftInReforecastStep(string budgetPlanId, string comment, bool notify)
    {
        var budget = await _budgetService.GetBudgetByPlanId(budgetPlanId);

        var isValid = _currentUserContext.IsInRoles(new[]
                          { AppRoles.SysAdmins, AppRoles.Admins }) ||
                      (_currentUserContext.IsInRole(AppRoles.Contributors) &&
                       _currentUserContext.UserId == budget.OwnerId);
        if (!isValid)
            throw new AccessDeniedException();

        await SetPlanStatus(budgetPlanId, BudgetPlanStatus.Draft);

        await AddChatMessages(budget.Id, BudgetPlanStatusAction.ReturnToDraft, comment);

        var parentManager = await _userService.GetAsync(budget.ParentManagerId);
        var body = MailTemplates.BudgetPlanRejected(budget.Title, comment);

        var listTo = new List<string> { parentManager.Email };
        if (notify)
        {
            var manager = await _userService.GetAsync(budget.ManagerId);
            listTo.Add(manager.Email);
        }

        await _mailService.QueueAsync(listTo,
            GroupedMailTemplates.BudgetPlanTemplates.Reforecast.Rejected.Subject,
            body);
    }
}
