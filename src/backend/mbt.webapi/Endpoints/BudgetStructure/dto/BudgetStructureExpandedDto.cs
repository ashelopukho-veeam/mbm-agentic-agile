// using System.Linq;
// using mbt.webapi.Domain.Entities;
// using UserProfile = mbt.webapi.Domain.Entities.UserProfile;
//
//
// namespace mbt.webapi.Endpoints.BudgetStructure.dto;
//
// public class BudgetStructureExpandedDto : BudgetStructureDto, IBaseItemExpanded
// {
//     public UserProfile Owner { get; set; }
//     public UserProfile ParentManager { get; set; }
//     public UserProfile Manager { get; set; }
//
//     public UserProfile CreatedByUser { get; set; }
//     public UserProfile ModifiedByUser { get; set; }
//
//     public static BudgetStructureExpandedDto FromBudgetExpanded(BudgetStructureExpanded budgetStructureExpanded)
//     {
//         var budgetStructureExpandedDto = new BudgetStructureExpandedDto()
//         {
//             Id = budgetStructureExpanded.Id,
//             //
//             Title = budgetStructureExpanded.Title,
//             CreatedBy = budgetStructureExpanded.CreatedBy,
//             Created = budgetStructureExpanded.Created,
//             ModifiedBy = budgetStructureExpanded.ModifiedBy,
//             Modified = budgetStructureExpanded.Modified,
//             //
//             Year = budgetStructureExpanded.Year,
//             Level1 = budgetStructureExpanded.Level1,
//             Level2 = budgetStructureExpanded.Level2,
//             Level3 = budgetStructureExpanded.Level3,
//             CostCenter = budgetStructureExpanded.CostCenter,
//
//             OwnerId = budgetStructureExpanded.OwnerId,
//             ParentManagerId = budgetStructureExpanded.ParentManagerId,
//             ManagerId = budgetStructureExpanded.ManagerId,
//             Status = budgetStructureExpanded.Status,
//             BudgetType = budgetStructureExpanded.BudgetType,
//             UseInCoupa = budgetStructureExpanded.UseInCoupa,
//             UseInTableau = budgetStructureExpanded.UseInTableau,
//             IsPaidMedia = budgetStructureExpanded.IsPaidMedia,
//             //
//             Owner = budgetStructureExpanded.Owner,
//             ParentManager = budgetStructureExpanded.ParentManager,
//             Manager = budgetStructureExpanded.Manager,
//             //
//             Committed = budgetStructureExpanded.Committed,
//             Sponsorship = budgetStructureExpanded.Sponsorship,
//             //
//             Plans = budgetStructureExpanded.Plans.Select(BudgetPlanDto.FromBudgetPlan).ToList(),
//             //
//             CreatedByUser = budgetStructureExpanded.CreatedByUser,
//             ModifiedByUser = budgetStructureExpanded.ModifiedByUser
//         };
//
//         return budgetStructureExpandedDto;
//     }
//
// }
