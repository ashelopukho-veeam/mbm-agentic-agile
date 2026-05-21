using System;

namespace mbt.webapi.Domain.Entities.Common;

public interface IAuditableItem : IBaseIdItem
{
    public string CreatedBy { get; set; }
    public DateTime Created { get; set; }

    public string ModifiedBy { get; set; }
    public DateTime Modified { get; set; }
}
