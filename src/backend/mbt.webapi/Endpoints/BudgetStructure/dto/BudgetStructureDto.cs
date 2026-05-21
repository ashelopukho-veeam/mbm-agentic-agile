using System.Collections.Generic;
using System.Linq;
using Budget = mbt.webapi.Domain.Entities.Budget;
using QuartersValues = mbt.webapi.Domain.Entities.QuartersValues;


namespace mbt.webapi.Endpoints.BudgetStructure.dto;

public class BudgetStructureDto : BaseItemDto
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

    public QuartersValues Committed { get; set; } = new();
    public QuartersValues Sponsorship { get; set; } = new();

    public List<BudgetPlanDto> Plans { get; set; }

    public static BudgetStructureDto FromBudget(Budget budget)
    {
        var dto = new BudgetStructureDto()
        {
            Id = budget.Id,
            //
            Title = budget.Title,
            CreatedBy = budget.CreatedBy,
            Created = budget.Created,
            ModifiedBy = budget.ModifiedBy,
            Modified = budget.Modified,
            //
            Year = budget.Year,
            Level1 = budget.Level1,
            Level2 = budget.Level2,
            Level3 = budget.Level3,
            CostCenter = budget.CostCenter,
            OwnerId = budget.OwnerId,
            ParentManagerId = budget.ParentManagerId,
            ManagerId = budget.ManagerId,
            Status = budget.Status,
            BudgetType = budget.BudgetType,
            UseInCoupa = budget.UseInCoupa,
            UseInTableau = budget.UseInTableau,
            IsPaidMedia = budget.IsPaidMedia,
            Committed = budget.Committed,
            Sponsorship = budget.Sponsorship,
            Plans = budget.Plans.Select(BudgetPlanDto.FromBudgetPlan).ToList()
        };

        return dto;
    }
}
