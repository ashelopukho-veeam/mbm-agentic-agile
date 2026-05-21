using mbt.webapi.Domain.Entities.Common;
using MongoDB.Bson;

namespace mbt.webapi.Domain.Entities;

public class BudgetPlanConfig : BaseIdItem
{
    public int CurrentBudgetPlanYear { get; set; } = 0;
    public string CurrentBudgetPlanName { get; set; } = "";


    public BudgetPlanConfig()
    {
        Id = ObjectId.GenerateNewId().ToString();
    }
}
