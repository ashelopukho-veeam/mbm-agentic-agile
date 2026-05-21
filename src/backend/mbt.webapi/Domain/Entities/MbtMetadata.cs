using mbt.webapi.Domain.Entities.Common;

namespace mbt.webapi.Domain.Entities;

public class TreeItem : BaseStringIdItem
{
    public string Title { get; set; }

    public string ParentId { get; set; }

    public string Value { get; set; }
}
