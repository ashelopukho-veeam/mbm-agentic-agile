using System.Collections.Generic;
using System.Threading.Tasks;
using mbt.webapi.Domain.Entities;

namespace mbt.webapi.Services.Interfaces;

public interface ITaskService : IBaseService
{
    Task<List<MbtTask>> GetByAssociatedItemId(string associatedItemId);
    Task CancelByAssociatedItemId(string associatedItemId);
    Task<List<MbtTask>> GetByStatus(string status);
    Task<MbtTask> GetAsync(string taskId);
    Task<List<MbtTask>> GetAsync();
    List<MbtTask> Get();
    MbtTask Get(string taskId);
    MbtTask Create(MbtTask task);
    void CancelTask(string taskId);
    Task Resolve(string taskId, string comment, string outcome);
}
