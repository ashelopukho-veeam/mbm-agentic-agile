using mbt.webapi.Domain.Entities.Common;

namespace mbt.webapi.Domain.Entities;

public class BudgetLevel : BaseItem
{
    public string ShortTitle { get; set; }
    public int Level { get; set; }
}
