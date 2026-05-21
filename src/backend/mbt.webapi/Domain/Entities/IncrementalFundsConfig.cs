using mbt.webapi.Domain.Entities.Common;

namespace mbt.webapi.Domain.Entities;

public class IncrementalFundsConfig : BaseStringIdItem
{
    public string WorkflowApproverId { get; set; }
}
