namespace mbt.webapi.Domain.Entities;

public class GroupedActivityExpanded : GroupedActivity, IBaseItemExpanded
{
    public UserProfile CreatedByUser { get; set; }
    public UserProfile ModifiedByUser { get; set; }
    public Budget Budget { get; set; }
}
