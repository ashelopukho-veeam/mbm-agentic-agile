#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using mbt.webapi.Services.Interfaces;
using Microsoft.Extensions.Hosting;

namespace mbt.webapi.Services;

public class RemoveOldItemsTimedService : IHostedService, IDisposable
{
    private Timer _timer = null!;

    private readonly IGroupActivitiesService _groupActivitiesService;

    public RemoveOldItemsTimedService(IGroupActivitiesService groupActivitiesService)
    {
        _groupActivitiesService = groupActivitiesService;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromHours(3));

        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        _groupActivitiesService.RemoveOldItems(TimeSpan.FromDays(7));
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _timer.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}
