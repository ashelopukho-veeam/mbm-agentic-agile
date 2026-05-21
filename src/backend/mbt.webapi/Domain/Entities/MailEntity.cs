using System.Collections.Generic;
using mbt.webapi.Domain.Entities.Common;

namespace mbt.webapi.Domain.Entities;

public class MailboxAddress
{
    public string Name { get; set; }
    public string Address { get; set; }
}

public class MailEntity : BaseIdItem
{
    private string _group;

    public string From { get; set; }
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }

    public string Group
    {
        get => _group ?? Subject;
        set => _group = value;
    }

    public NotificationStatus Status { get; set; } = NotificationStatus.Unprocessed;
}

public enum NotificationStatus
{
    Unprocessed,
    Processed,
    ReadyToSend,
    Sent,
    Failed
}
