using JetBrains.Annotations;
using Budget = mbt.webapi.Domain.Entities.Budget;
using UserProfile = mbt.webapi.Domain.Entities.UserProfile;


namespace mbt.webapi.Endpoints.IncrementalFunds.dto;

[PublicAPI]
public class IncrementalFundExpandedDto : IncrementalFundDto
{
    public UserProfile CreatedByUser { get; set; }
    public UserProfile ModifiedByUser { get; set; }
    public Budget ToBudget { get; set; }
}

[PublicAPI]
public class IncrementalFundWithDetailsDto : IncrementalFundDto
{
    public UserProfile CreatedByUser { get; set; }
    public UserProfile ModifiedByUser { get; set; }
    public string ToBudgetName { get; set; }
}
