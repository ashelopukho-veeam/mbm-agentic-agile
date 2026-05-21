using mbt.webapi.Domain.Entities.Common;

namespace mbt.webapi.Domain.Entities;

public class NotificationSchedule : BaseIdItem
{
    public string Name { get; set; }
    public string Cron { get; set; }
}
