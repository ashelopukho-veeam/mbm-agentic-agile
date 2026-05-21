namespace mbt.webapi.Domain.Entities;

public class IncrementalFundExpanded : IncrementalFund, IBaseItemExpanded
{
    public Budget ToBudget { get; set; }
    public UserProfile CreatedByUser { get; set; }
    public UserProfile ModifiedByUser { get; set; }
}
