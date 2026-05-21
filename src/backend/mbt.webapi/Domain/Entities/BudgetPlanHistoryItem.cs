using System;
using mbt.webapi.Domain.Entities.Common;
using MongoDB.Bson;

namespace mbt.webapi.Domain.Entities;

public class BudgetPlanHistoryItem : BudgetPlan, IHistoryItem, IAuditableItem
{
    public string OriginalId { get; set; }

    public BudgetPlanHistoryItem()
    {
        Id = ObjectId.GenerateNewId().ToString();
    }

    public string CreatedBy { get; set; }
    public DateTime Created { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime Modified { get; set; }
}
