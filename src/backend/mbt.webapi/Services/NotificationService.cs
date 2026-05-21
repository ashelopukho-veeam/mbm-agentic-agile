using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Jobs;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using Quartz;

namespace mbt.webapi.Services;

[UsedImplicitly]
public class NotificationService : INotificationService
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IDbBaseRepository<NotificationEvent> _eventsRepository;

    public NotificationService(ISchedulerFactory schedulerFactory, IDbBaseRepository<NotificationEvent> eventsRepository)
    {
        _schedulerFactory = schedulerFactory;
        _eventsRepository = eventsRepository;
    }

    private void AddNotificationJob()
    {
        // add job that will be executed every 5 minute
        var interval = 1;
        var job = JobBuilder.Create<EventsJob>().WithIdentity("job1").Build();
        var trigger = TriggerBuilder.Create().WithIdentity(EventsJob.Key.Name).StartNow()
            .WithSimpleSchedule(x => x.WithIntervalInMinutes(interval).RepeatForever()).Build();
        var scheduler = _schedulerFactory.GetScheduler().Result;
        scheduler.ScheduleJob(job, trigger);
    }

    public void Init()
    {
        AddNotificationJob();
    }

    public async Task<IEnumerable<NotificationEvent>> GetNotificationEvents()
    {
        var events = await _eventsRepository.GetAsync();
        return events;
    }

    public async Task UpdateNotificationEvent(NotificationEvent notificationEvent)
    {
        await _eventsRepository.UpdateAsync(notificationEvent);
    }

    public Task RemoveAsync(string id)
    {
        return _eventsRepository.RemoveAsync(id);
    }

    public Task AddAsync(NotificationEvent notificationEvent)
    {
        return _eventsRepository.CreateAsync(notificationEvent);
    }
}
