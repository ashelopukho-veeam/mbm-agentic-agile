using mbt.webapi.Domain.Entities.Common;

namespace mbt.webapi.Domain.Entities;

public class PaidMediaTeamApprover : BaseIdItem
{
    public string Team { get; set; }
    public string ApproverId { get; set; }
}

public class PaidMediaTeamApproverExpanded : BaseIdItem  {
    public string Team { get; set; }
    public string ApproverId { get; set; }
    public UserProfile Approver { get; set; }
}
