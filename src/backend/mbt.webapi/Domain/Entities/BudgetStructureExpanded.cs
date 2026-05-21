using System.Collections.Generic;
using JetBrains.Annotations;
using mbt.webapi.Domain.Entities.Common;

namespace mbt.webapi.Domain.Entities;

[PublicAPI]
public class BudgetStructureExpanded : BaseItem, IBaseItemExpanded
{
    public int Year { get; set; }

    public string Level1 { get; set; }

    public string Level2 { get; set; }

    public string Level3 { get; set; }

    public string CostCenter { get; set; }

    public string OwnerId { get; set; }
    public string ParentManagerId { get; set; }
    public string ManagerId { get; set; }

    public string Status { get; set; }
    public string BudgetType { get; set; }

    public bool UseInCoupa { get; set; }
    public bool UseInTableau { get; set; }
    public bool IsPaidMedia { get; set; }

    public UserProfile Owner { get; set; }
    public UserProfile Manager { get; set; }
    public UserProfile ParentManager { get; set; }

    public QuartersValues Committed { get; set; } = new();
    public QuartersValues Sponsorship { get; set; } = new();

    public List<BudgetPlan> Plans { get; set; }

    public bool IsDeleted { get; set; }
    public UserProfile CreatedByUser { get; set; }
    public UserProfile ModifiedByUser { get; set; }
}
