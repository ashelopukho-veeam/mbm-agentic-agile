using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;

namespace mbt.webapi.Services;

[UsedImplicitly]
public class TaskService : ITaskService
{
    private readonly IDbBaseRepository<MbtTask> _taskRepository;

    public TaskService(IDbBaseRepository<MbtTask> taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public Task<List<MbtTask>> GetByAssociatedItemId(string associatedItemId)
    {
        return _taskRepository.FindAsync(t => t.AssociatedItemId == associatedItemId);
    }

    public async Task CancelByAssociatedItemId(string associatedItemId)
    {
        var tasks = await GetByAssociatedItemId(associatedItemId);
        foreach (var task in tasks)
        {
            if (!string.IsNullOrEmpty(task.Status) && task.Status != MbtTaskStatus.Pending) continue;

            task.Status = MbtTaskStatus.Canceled;
            await _taskRepository.UpdateAsync(task);
        }
    }

    public Task<List<MbtTask>> GetByStatus(string status)
    {
        return _taskRepository.FindAsync(t => t.Status == status);
    }

    public Task<MbtTask> GetAsync(string taskId)
    {
        return _taskRepository.GetAsync(taskId);
    }

    public Task<List<MbtTask>> GetAsync()
    {
        return _taskRepository.GetAsync();
    }

    public List<MbtTask> Get()
    {
        return _taskRepository.Get();
    }

    public MbtTask Create(MbtTask task)
    {
        return _taskRepository.Create(task);
    }

    public MbtTask Get(string taskId)
    {
        return _taskRepository.Get(taskId);
    }

    public void CancelTask(string taskId)
    {
        var task = _taskRepository.Get(taskId);
        task.Status = MbtTaskStatus.Canceled;
        _taskRepository.Update(task);
    }

    public async Task Resolve(string taskId, string comment, string outcome)
    {
        var task = await _taskRepository.GetAsync(taskId);
        task.Comment = comment;
        task.Outcome = outcome;
        task.Status = "Resolved";

        await _taskRepository.UpdateAsync(task);
    }
}
