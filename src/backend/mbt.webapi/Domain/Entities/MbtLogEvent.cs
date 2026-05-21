using mbt.webapi.Domain.Entities.Common;

namespace mbt.webapi.Domain.Entities;

public class MbtLogEvent : BaseItem
{
    public string Message { get; set; }
    public string EventId { get; set; }
    public string AssiciatedItemId { get; set; }
}
