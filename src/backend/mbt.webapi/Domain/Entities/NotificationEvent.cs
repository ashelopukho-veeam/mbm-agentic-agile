using System;
using mbt.webapi.Domain.Entities.Common;

namespace mbt.webapi.Domain.Entities;

public class NotificationEvent : BaseIdItem
{
    public DateTime EventDate { get; set; }
    public bool IsSent { get; set; }
}
