using JetBrains.Annotations;
using Budget = mbt.webapi.Domain.Entities.Budget;

namespace mbt.webapi.Endpoints.BudgetStructure.dto;

[PublicAPI]
public class BudgetNamesByYearDto
{
    public string Id { get; set; }

    public string Title { get; set; }

    public string Status { get; set; }

    public static BudgetNamesByYearDto FromBudgetNamesByYear(Budget obj)
    {
        return new BudgetNamesByYearDto()
        {
            Id = obj.Id,
            Title = obj.Title,
            Status = obj.Status
        };
    }
}
