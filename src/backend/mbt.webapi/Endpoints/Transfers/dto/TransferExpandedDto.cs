using JetBrains.Annotations;
using Budget = mbt.webapi.Domain.Entities.Budget;
using UserProfile = mbt.webapi.Domain.Entities.UserProfile;


namespace mbt.webapi.Endpoints.Transfers.dto;

[PublicAPI]
public class TransferExpandedDto : TransferDto
{
    public UserProfile CreatedByUser { get; set; }
    public UserProfile ModifiedByUser { get; set; }
    public Budget FromBudget { get; set; }
    public Budget ToBudget { get; set; }
}

[PublicAPI]
public class TransferWithDetailsDto : TransferDto
{
    public UserProfile CreatedByUser { get; set; }
    public UserProfile ModifiedByUser { get; set; }
    public string FromBudgetName { get; set; }
    public string ToBudgetName { get; set; }
}
