using System.Collections.Generic;
using System.Linq;
using mbt.webapi.Domain.Entities.Common;
using MongoDB.Bson.Serialization.Attributes;

namespace mbt.webapi.Domain.Entities;

[BsonIgnoreExtraElements]
public class Budget : BaseItem, IIsDeleted
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

    public List<BudgetPlan> Plans { get; set; } = new();

    public QuartersValues Committed { get; set; } = new();
    public QuartersValues Sponsorship { get; set; } = new();

    public BudgetPlan GetBudgetPlanById(string id)
    {
        return Plans.FirstOrDefault(p => p.Id == id);
    }


    public BudgetPlan GetPlanByQuarter(string quarter)
    {
        return Plans.FirstOrDefault(p => p.Quarter == quarter);
    }

    public bool IsDeleted { get; set; }
}
