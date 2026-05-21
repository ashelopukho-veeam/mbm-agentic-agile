namespace mbt.webapi.Domain.Entities;

public class PaidMediaSetWithLinkedItemTitle : PaidMediaSet
{
    public string LinkedItemTitle { get; set; }
    public UserProfile CreatedByUser { get; set; }
    public UserProfile ModifiedByUser { get; set; }
}
