using BudgetLevel = mbt.webapi.Domain.Entities.BudgetLevel;


namespace mbt.webapi.Endpoints.BudgetLevels;

public class BudgetLevelDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string ShortTitle { get; set; }
    public int Level { get; set; }

    public static BudgetLevelDto FromBudgetLevel(BudgetLevel level)
    {
        return new BudgetLevelDto
        {
            Id = level.Id,
            Title = level.Title,
            ShortTitle = level.ShortTitle,
            Level = level.Level
        };
    }
}
