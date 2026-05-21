using System.Threading.Tasks;
using mbt.webapi.BuiltIn;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Endpoints.BudgetStructure.dto;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;

namespace mbt.webapi.Services.Budgets;

public interface IBudgetConstructionService : IBaseService
{
    Task<Budget> BuildBudget(CreateBudgetStructureRequest request);

    Task<string> BuildBudgetTitle(string level1Title, string level2Title, string level3Title, string costCenterTitle,
        int year);
}

public class BudgetConstructionService : IBudgetConstructionService
{
    private readonly IDbBaseRepository<CostCenter> _costCenterRepository;
    private readonly IDbBaseRepository<BudgetLevel> _levelRepository;

    public BudgetConstructionService(IDbBaseRepository<CostCenter> costCenterRepository,
        IDbBaseRepository<BudgetLevel> levelRepository)
    {
        _costCenterRepository = costCenterRepository;
        _levelRepository = levelRepository;
    }

    public async Task<Budget> BuildBudget(CreateBudgetStructureRequest request)
    {
        var budget = new Budget
        {
            BudgetType = request.BudgetType,
            Level1 = request.Level1,
            Level2 = request.Level2,
            Level3 = request.Level3,
            CostCenter = request.CostCenter,
            Year = request.Year,
            OwnerId = request.OwnerId.ToString(),
            ParentManagerId = request.ParentManagerId.ToString(),
            ManagerId = request.ManagerId.ToString(),
            IsPaidMedia = request.IsPaidMedia,
            Status = request.Status,
            UseInCoupa = request.Status == BudgetStatus.ApprovedPlaceholder,
            UseInTableau = request.Status == BudgetStatus.ApprovedPlaceholder,
            Committed = new QuartersValues(),
            Sponsorship = new QuartersValues(),
        };

        var title = await BuildBudgetTitle(request.Level1, request.Level2, request.Level3, request.CostCenter,
            request.Year);

        budget.Title = title;

        return budget;
    }

    public async Task<string> BuildBudgetTitle(string level1Title, string level2Title, string level3Title,
        string costCenterTitle, int year)
    {
        const int minYear = 2000;
        if (year < minYear)
            throw new ApiException("Year must be greater than 2000");

        var level1ShortTitle = await GetBudgetLevelShortTitle(level1Title, 1);
        var level2ShortTitle = await GetBudgetLevelShortTitle(level2Title, 2);
        var level3ShortTitle = await GetBudgetLevelShortTitle(level3Title, 3);

        var costCenterShortTitle = await GetCostCenterShortTitle(costCenterTitle);

        var title =
            $"{level1ShortTitle} - {level2ShortTitle} - {level3ShortTitle} - {costCenterShortTitle} {year.ToString()[2..]}";

        if (title.Length > BuiltInConstants.BudgetTitleLengthLimit)
            throw new ApiException(
                $"Unable to build budget title. Title length limit is {BuiltInConstants.BudgetTitleLengthLimit} characters");

        return title;
    }

    private async Task<string> GetBudgetLevelShortTitle(string title, int level)
    {
        var budgetLevel = await _levelRepository.FindOneAsync(l => l.Title == title && l.Level == level);
        if (budgetLevel == null)
            throw new ApiException($"Unable to find level {level} with title {title}");

        return budgetLevel.ShortTitle;
    }

    private async Task<string> GetCostCenterShortTitle(string costCenterTitle)
    {
        var costCenter = await _costCenterRepository.FindOneAsync(c => c.Title == costCenterTitle);
        if (costCenter == null)
            throw new ApiException($"Unable to find cost center with title {costCenterTitle}");

        return costCenter.ShortTitle;
    }
}
