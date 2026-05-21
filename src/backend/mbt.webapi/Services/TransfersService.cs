using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Jobs;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using mbt.webapi.Shared;
using mbt.webapi.Utils;
using MongoDB.Driver;
using WorkflowCore.Interface;

namespace mbt.webapi.Services;

[UsedImplicitly]
public class TransfersService : ITransfersService
{
    private readonly IApiService _apiService;
    private readonly IWorkflowController _workflowController;
    private readonly ITaskService _taskService;
    private readonly IChatService _chatService;
    private readonly ITransfersRepository _transfersRepository;
    private readonly IBudgetRepository _budgetsRepository;
    private readonly ICurrentUserContext _userContext;
    private readonly IMailService _mailService;
    private readonly IUserService _userService;

    public TransfersService(IApiService apiService, ITransfersRepository transfersRepository,
        IWorkflowController workflowController, ITaskService taskService, IChatService chatService,
        ICurrentUserContext userContext, IMailService mailService, IBudgetRepository budgetsRepository,
        IUserService userService)
    {
        _apiService = apiService;
        _transfersRepository = transfersRepository;
        _workflowController = workflowController;
        _taskService = taskService;
        _chatService = chatService;
        _userContext = userContext;
        _mailService = mailService;
        _budgetsRepository = budgetsRepository;
        _userService = userService;
    }


    private FilterDefinition<TransferExpanded> GetFilterDefinition(int year, string plan)
    {
        var filterBuilder = Builders<TransferExpanded>.Filter;
        var yearFilter = filterBuilder.Eq(x => x.Year, year);

        var resultFilter = yearFilter;

        if (!string.IsNullOrWhiteSpace(plan))
        {
            var planFilter = filterBuilder.Eq(x => x.Plan, plan);
            resultFilter &= planFilter;
        }

        return resultFilter;
    }

    public async Task<List<TransferExpanded>> GetExpanded(int year, string plan)
    {
        var filterDefinition = GetFilterDefinition(year, plan);

        var result =
            await _transfersRepository.GetExpanded(filterDefinition);
        return result;
    }


    public Task<TransferExpanded> GetExpanded(string id)
    {
        return _transfersRepository.GetByIdExpanded(id);
    }

    public Task<List<Transfer>> GetTransfersForBudget(string budgetId)
    {
        return _transfersRepository.FindAsync(
            t => t.FromBudgetId == budgetId || t.ToBudgetId == budgetId);
    }


    public Task<Transfer> GetAsync(string transferId)
    {
        return _transfersRepository.GetAsync(transferId);
    }

    public void Update(Transfer transfer)
    {
        _transfersRepository.Update(transfer);
    }


    public Transfer Get(string transferId)
    {
        return _transfersRepository.Get(transferId);
    }



    public async Task<Transfer> ExpireTransfer(string transferId)
    {
        var transfer = await GetAsync(transferId);
        if (transfer == null) throw new ApiException(ErrorMessages.TransferNotFound);
        if (transfer.Status != TransferStatus.PendingApproval)
            throw new ApiException(ErrorMessages.TransferExpireWrongStatus);

        // terminate workflow
        var wfId = transfer.WorkflowId;
        if (!string.IsNullOrEmpty(wfId))
            await _workflowController.TerminateWorkflow(wfId);

        // cancel associated tasks
        var fromBudget = await _budgetsRepository.GetAsync(transfer.FromBudgetId);
        var toBudget = await _budgetsRepository.GetAsync(transfer.ToBudgetId);

        var tasks = await _taskService.GetByAssociatedItemId(transfer.Id);
        await _taskService.CancelByAssociatedItemId(transfer.Id);

        transfer.IsExpired = true;
        transfer.Status = TransferStatus.Canceled;
        await _transfersRepository.UpdateAsync(transfer);

        var usersToNotify = new List<string>();
        foreach (var task in tasks)
        {
            var user = await _userService.GetAsync(task.AssignedTo);
            if (user?.Email == null) continue;
            usersToNotify.Add(user.Email);
        }

        var host = (await _apiService.GetAppConfigAsync()).ClientHostUrl;

        var displayFormUrl = UrlUtils.Combine(host, SpaRoutes.TransfersViewItem(transfer.Id));
        await _mailService.QueueAsync(
            usersToNotify,
            GroupedMailTemplates.Transfers.Expired.Subject,
            MailTemplates.TransferExpiredBody(
                displayFormUrl,
                transfer.Title,
                fromBudget.Title,
                toBudget.Title,
                transfer.Amount.ToString(CultureInfo.InvariantCulture)
            ));


        await _chatService.AddSystemChatMessageAsync(transferId,
            ChatSystemMessages.TransferExpired + _userContext.UserName);

        return transfer;
    }

    public async Task<Budget> ValidateBudgetForTransfer(string budgetId, Period transferPeriod)
    {
        var filter = Builders<Budget>.Filter.Where(b =>
            b.Id == budgetId &&
            b.Year == transferPeriod.Year &&
            b.Status == BudgetStatus.Approved);

        var budget = await _budgetsRepository.FindOneAsync(filter);

        if (budget == null)
            throw new ArgumentException(ErrorMessages.ItemNotFound("Budget"));

        return budget;
    }
}
