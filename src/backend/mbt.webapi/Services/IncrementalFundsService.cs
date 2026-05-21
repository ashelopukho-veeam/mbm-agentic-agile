using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Jobs;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using mbt.webapi.Utils;
using MongoDB.Driver;
using WorkflowCore.Interface;

namespace mbt.webapi.Services;

[UsedImplicitly]
public class IncrementalFundsService : IIncrementalFundsService
{
    private readonly IApiService _apiService;
    private readonly IIncrementalFundsRepository _incrementalFundsRepository;
    private readonly IBudgetRepository _budgetsRepository;
    private readonly IDbBaseRepository<IncrementalFundsConfig> _incrementalFundsConfigRepository;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IWorkflowController _workflowController;
    private readonly ITaskService _taskService;
    private readonly IUserService _userService;
    private readonly IChatService _chatService;
    private readonly IMailService _mailService;
    private readonly IValidator<IWithPaidMediaData> _paidMediaFieldsValidator;

    public IncrementalFundsService(
        IApiService apiService,
        IDbBaseRepository<IncrementalFundsConfig> incrementalFundsConfigRepository,
        ICurrentUserContext currentUserContext,
        IWorkflowController workflowController, ITaskService taskService, IUserService userService,
        IChatService chatService, IMailService mailService, IIncrementalFundsRepository incrementalFundsRepository,
        IBudgetRepository budgetsRepository, IValidator<IWithPaidMediaData> paidMediaFieldsValidator)
    {
        _apiService = apiService;

        _incrementalFundsConfigRepository = incrementalFundsConfigRepository;
        _currentUserContext = currentUserContext;
        _workflowController = workflowController;
        _taskService = taskService;
        _userService = userService;
        _chatService = chatService;
        _mailService = mailService;
        _incrementalFundsRepository = incrementalFundsRepository;
        _budgetsRepository = budgetsRepository;
        _paidMediaFieldsValidator = paidMediaFieldsValidator;
    }

    private FilterDefinition<IncrementalFundExpanded> GetFilterDefinition(int year, string plan)
    {
        var filterBuilder = Builders<IncrementalFundExpanded>.Filter;
        var yearFilter = filterBuilder.Eq(x => x.Year, year);

        var resultFilter = yearFilter;

        if (!string.IsNullOrWhiteSpace(plan))
        {
            var planFilter = filterBuilder.Eq(x => x.Plan, plan);
            resultFilter &= planFilter;
        }

        return resultFilter;
    }

    public async Task<List<IncrementalFundExpanded>> GetExpanded(int year, string plan)
    {
        var filterDefinition = GetFilterDefinition(year, plan);
        var result = await _incrementalFundsRepository.GetExpanded(filterDefinition);

        return result;
    }

    public async Task<IncrementalFundExpanded> GetExpanded(string id)
    {
        var result = await _incrementalFundsRepository.GetByIdExpanded(id);
        return result;
    }

    public async Task<List<IncrementalFund>> GetByBudgetIdAsync(string budgetId)
    {
        var result = await _incrementalFundsRepository.FindAsync(f => f.ToBudgetId == budgetId);

        return result;
    }

    public Task<List<IncrementalFund>> GetByYearAsync(int year)
    {
        return _incrementalFundsRepository.FindAsync(f => f.Year == year);
    }

    public IncrementalFund Create(IncrementalFund obj)
    {
        var config = _apiService.GetCommonConfig();
        if (!config.AllowCreateTransfers)
            throw new Exception($"CollectionName: {CollectionNames.IncrementalFunds}. AllowCreateTransfers: false");

        return _incrementalFundsRepository.Create(obj);
    }

    public Task<IncrementalFund> CreateAsync(IncrementalFund obj)
    {
        var config = _apiService.GetCommonConfig();
        if (!config.AllowCreateTransfers)
            throw new Exception($"CollectionName: {CollectionNames.IncrementalFunds}. AllowCreateTransfers: false");

        return _incrementalFundsRepository.CreateAsync(obj);
    }


    public Task<IncrementalFund> GetAsync(string requestId)
    {
        return _incrementalFundsRepository.GetAsync(requestId);
    }

    public Task UpdateAsync(IncrementalFund incrementalFund)
    {
        return _incrementalFundsRepository.UpdateAsync(incrementalFund);
    }

    public Task RemoveAsync(IncrementalFund itemToDelete)
    {
        return _incrementalFundsRepository.RemoveAsync(itemToDelete.Id);
    }


    public IncrementalFund Get(string incrementalFundId)
    {
        return _incrementalFundsRepository.Get(incrementalFundId);
    }

    public void Update(IncrementalFund incrementalFund)
    {
        _incrementalFundsRepository.Update(incrementalFund);
    }

    public async Task SetConfig(IncrementalFundsConfig config)
    {
        var currentConfig = await _incrementalFundsConfigRepository.FindOneAsync(_ => true);
        if (currentConfig == null)
        {
            config.Id = Guid.NewGuid().ToString();
            await _incrementalFundsConfigRepository.CreateAsync(config);
        }
        else
        {
            config.Id = currentConfig.Id;
            await _incrementalFundsConfigRepository.UpdateAsync(config);
        }
    }

    public Task<IncrementalFundsConfig> GetConfig()
    {
        return _incrementalFundsConfigRepository.FindOneAsync(_ => true);
    }

    public async Task<IncrementalFund> ExpireIncrementalFund(string incrementalFundId)
    {
        var incrementalFund = await _incrementalFundsRepository.GetAsync(incrementalFundId);

        if (incrementalFund == null)
            throw new ApiException(ErrorMessages.IncrementalFundNotFound);

        if (incrementalFund.Status != IncrementalFundStatus.PendingApproval)
            throw new ApiException(ErrorMessages.IncrementalFundExpireWrongStatus);

        // terminate workflow
        var wfId = incrementalFund.WorkflowId;
        if (!string.IsNullOrEmpty(wfId))
            await _workflowController.TerminateWorkflow(wfId);

        // cancel associated tasks
        var toBudget = await _budgetsRepository.GetAsync(incrementalFund.ToBudgetId);
        var tasks = await _taskService.GetByAssociatedItemId(incrementalFund.Id);
        var pendingTasks = tasks.FindAll(t => t.Status == MbtTaskStatus.Pending);
        await _taskService.CancelByAssociatedItemId(incrementalFund.Id);

        incrementalFund.IsExpired = true;
        incrementalFund.Status = IncrementalFundStatus.Canceled;
        await UpdateAsync(incrementalFund);

        var usersToNotify = new List<string>();
        foreach (var task in pendingTasks)
        {
            var user = await _userService.GetAsync(task.AssignedTo);
            if (user?.Email == null) continue;
            usersToNotify.Add(user.Email);
        }

        var host = (await _apiService.GetAppConfigAsync()).ClientHostUrl;

        var displayFormUrl = UrlUtils.Combine(host, SpaRoutes.IncrementalFundsViewItem(incrementalFund.Id));
        await _mailService.QueueAsync(
            usersToNotify,
            GroupedMailTemplates.IncrementalFunds.Expired.Subject,
            MailTemplates.IncrementalFundExpiredBody(
                incrementalFund.Title,
                toBudget.Title,
                incrementalFund.Amount.ToString(CultureInfo.InvariantCulture),
                displayFormUrl
            ));

        await _chatService.AddSystemChatMessageAsync(incrementalFundId,
            ChatSystemMessages.IncrementalFundExpired + _currentUserContext.UserName);

        return incrementalFund;
    }

    public async Task<IncrementalFund> CancelIncrementalFund(string incrementalFundId)
    {
        var incrementalFund = await GetAsync(incrementalFundId);

        if (incrementalFund == null) throw new ApiException(ErrorMessages.IncrementalFundNotFound);

        // validate Admin or Author
        var isAdmin = _currentUserContext.IsInRole(AppRoles.Admins) || _currentUserContext.IsInRole(AppRoles.SysAdmins);
        var isAuthor = _currentUserContext.UserId == incrementalFund.CreatedBy;

        if (!isAdmin && !isAuthor)
            throw new AccessDeniedException();

        if (incrementalFund.Status != IncrementalFundStatus.Draft && incrementalFund.Status !=
            IncrementalFundStatus.PendingApproval)
            throw new ApiException(ErrorMessages.IncrementalFundCancelWrongStatus);


        // terminate workflow
        if (!string.IsNullOrEmpty(incrementalFund.WorkflowId))
        {
            try
            {
                await _workflowController.TerminateWorkflow(incrementalFund.WorkflowId);
            }
            catch (Exception)
            {
                // ignore
            }
        }


        // cancel associated tasks
        await _taskService.CancelByAssociatedItemId(incrementalFund.Id);

        incrementalFund.Status = IncrementalFundStatus.Canceled;
        await UpdateAsync(incrementalFund);

        var currentUserName = _currentUserContext.UserName;
        var author = await _userService.GetAsync(incrementalFund.CreatedBy);

        await _chatService.AddSystemChatMessageAsync(incrementalFund.Id,
            isAuthor
                ? $"Canceled by {currentUserName}"
                : $"Canceled by {currentUserName} on behalf of {author.DisplayName}");

        return incrementalFund;
    }


    public async Task<Budget> ValidateAndFetchBudgetAsync(string toBudgetId)
    {
        var lastFinalizedPeriod = await _apiService.GetLastFinalizedPeriod();
        var filter = Builders<Budget>.Filter.Where(b =>
            b.Id == toBudgetId &&
            b.Year == lastFinalizedPeriod.Year &&
            b.Status == BudgetStatus.Approved);

        var toBudget = await _budgetsRepository.FindOneAsync(filter);
        if (toBudget == null)
            throw new ApiException("Budget not found");

        return toBudget;
    }

    public async Task ValidateAndSetPaidMediaFieldsAsync(IWithPaidMediaData request,
        IncrementalFund incrementalFund,
        bool isPaidMedia,
        CancellationToken cancellationToken)
    {
        if (isPaidMedia)
        {
            await _paidMediaFieldsValidator.ValidateAndThrowAsync(request, cancellationToken);
        }

        PaidMediaHelper.SetPaidMediaFields(incrementalFund, isPaidMedia ? request : null);
    }
}
