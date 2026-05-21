using System.Collections.Generic;
using System.Threading.Tasks;
using mbt.webapi.Domain.Entities;

namespace mbt.webapi.Services.Interfaces;

public interface INotificationService : IBaseService
{
    void Init();

    Task<IEnumerable<NotificationEvent>> GetNotificationEvents();
    Task UpdateNotificationEvent(NotificationEvent notificationEvent);
    Task RemoveAsync(string id);
    Task AddAsync(NotificationEvent notificationEvent);
}
