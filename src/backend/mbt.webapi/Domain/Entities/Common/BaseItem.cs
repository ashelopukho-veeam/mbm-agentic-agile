using System;

namespace mbt.webapi.Domain.Entities.Common;

public class BaseItem : BaseIdItem, IAuditableItem
{
    public string Title { get; set; }

    public string CreatedBy { get; set; }
    public DateTime Created { get; set; }

    public string ModifiedBy { get; set; }
    public DateTime Modified { get; set; }
}
