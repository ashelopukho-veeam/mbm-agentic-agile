using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Services;
using mbt.webapi.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;

namespace mbt.webapi.Jobs;

[UsedImplicitly]
public class EventsJob : IJob
{
    public static readonly JobKey Key = new("EventsJob");

    private readonly INotificationService _notificationService;
    private readonly ILogger<EventsJob> _logger;

    private readonly ITaskService _taskService;
    private readonly IMailService _mailService;
    private readonly IApiService _apiService;
    private readonly IUserService _userService;

    public EventsJob(INotificationService notificationService, ILogger<EventsJob> logger, IApiService apiService,
        IMailService mailService, ITaskService taskService, IUserService userService)
    {
        _notificationService = notificationService;
        _logger = logger;
        _apiService = apiService;
        _mailService = mailService;
        _taskService = taskService;
        _userService = userService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("EventsJob started");

        var notificationEvents = await _notificationService.GetNotificationEvents();
        foreach (var notificationEvent in notificationEvents)
        {
            if (notificationEvent.EventDate > DateTime.UtcNow || notificationEvent.IsSent) continue;

            await SendTransferIncFundNotification();
            notificationEvent.IsSent = true;
            await _notificationService.UpdateNotificationEvent(notificationEvent);
        }

        _logger.LogInformation("EventsJob finished");
    }

    private async Task SendTransferIncFundNotification()
    {
        var tasksInPendingStatus = await _taskService.GetByStatus(MbtTaskStatus.Pending);

        var assignedUserIds = tasksInPendingStatus.Select(t => t.AssignedTo).Distinct().ToList();

        if (assignedUserIds.Count == 0)
            return;

        var appConfig = await _apiService.GetAppConfigAsync();
        var tasksUrl = appConfig.ClientHostUrl.Trim('/') + "/tasks";

        var linkToApprovalsList = $"<a href='{tasksUrl}' >MBM<a/>";

        var subject = "Requests waiting for your approval";
        var body =
            $"You are receiving this alert because you have transfer/incremental fund requests in {linkToApprovalsList} " +
            "that are waiting for your approval. Please, approve or reject them as soon as possible, " +
            "before the Marketing Operations close the transfers and incremental funds and open the reforecasting. " +
            "Once the reforecasting opens, the unapproved transfers and incremental funds will be canceled.";


        foreach (var userId in assignedUserIds)
        {
            var userEmail = (await _userService.GetAsync(userId)).Email;
            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                await _mailService.QueueAsync(userEmail, subject, body);
            }
        }
    }
}
