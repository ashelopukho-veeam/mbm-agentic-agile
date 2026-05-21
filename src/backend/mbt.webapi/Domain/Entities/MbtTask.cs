using mbt.webapi.Domain.Entities.Common;

namespace mbt.webapi.Domain.Entities;

public class MbtTask : BaseItem
{
    public string Type { get; set; }
    public string AssignedTo { get; set; }
    public string Outcome { get; set; }
    public string Status { get; set; }
    public string Comment { get; set; }

    public string AssociatedItemId { get; set; }
    public string AssociatedItemUrl { get; set; }
    public string Details { get; set; }
}

public class MbtTaskExpanded : MbtTask
{
    public UserProfile AssignedToUser { get; set; }
    public UserProfile CreatedByUser { get; set; }
    public UserProfile ModifiedByUser { get; set; }
}

public static class MbtTaskStatus
{
    public const string Approved = "Approved";
    public const string Rejected = "Rejected";
    public const string SendBackToEdit = "SendBackToEdit";
    public const string Pending = "Pending";


    public const string Canceled = "Canceled";
}

public static class MbtTaskResolveAction
{
    public const string Approve = "Approve";
    public const string Reject = "Reject";
    public const string SendBackToEdit = "SendBackToEdit";
}
