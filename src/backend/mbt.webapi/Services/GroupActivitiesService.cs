using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Endpoints.GroupedActivities;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;

namespace mbt.webapi.Services;

[UsedImplicitly]
public class GroupActivitiesService : IGroupActivitiesService
{
    private readonly ICurrencyService _currencyService;

    private readonly IGroupedActivityRepository _groupedActivitiesRepository;
    private readonly IDbBaseRepository<DictionaryDocument> _dictionaryRepository;

    public GroupActivitiesService(ICurrencyService currencyService,
        IGroupedActivityRepository groupedActivitiesRepository,
        IDbBaseRepository<DictionaryDocument> dictionaryRepository)
    {
        _currencyService = currencyService;
        _groupedActivitiesRepository = groupedActivitiesRepository;
        _dictionaryRepository = dictionaryRepository;
    }

    public async Task<List<GroupedActivity>> GetByBudgetPlanId(string id)
    {
        var result = await _groupedActivitiesRepository
            .FindAsync(g => g.BudgetPlanId == id);

        return result;
    }

    public async Task<List<GroupedActivityExpanded>> GetByBudgetPlanIdExpanded(string id)
    {
        var filter = new ExpressionFilterDefinition<GroupedActivityExpanded>(g => g.BudgetPlanId == id);
        var result = await _groupedActivitiesRepository.GetExpanded(filter);

        return result;
    }

    public async Task<List<GroupedActivity>> GetByBudgetIdAsync(string id)
    {
        var result = await _groupedActivitiesRepository.FindAsync(g =>
            g.BudgetId == id);

        return result;
    }

    private static double CalculateNetPlannedAmount(double plannedAmount, double plannedSponsorship, double rate)
    {
        return (plannedAmount - plannedSponsorship) * rate;
    }

    public async Task ImportFromCsv(Budget budget, string budgetPlanId, IFormFile formFile)
    {
        using var reader = new StreamReader(formFile.OpenReadStream());
        using var csv = new CsvReader(reader, CsvConstants.DefaultCsvConfiguration);
        csv.Context.RegisterClassMap<GroupedActivityCsvMap>();
        var records = csv.GetRecords<GroupedActivity>().ToList();

        var currencyRatesByYear = await _currencyService.GetByYear(budget.Year);

        foreach (var record in records)
        {
            record.BudgetPlanId = budgetPlanId;
            record.BudgetId = budget.Id;
            record.IsDeleted = false;
            var currencyRate = currencyRatesByYear.FirstOrDefault(c => c.Title == record.LocalCurrency);

            if (currencyRate == null) throw new ApiException("Can't find currency rate for: " + record.LocalCurrency);

            record.NetPlannedAmount =
                CalculateNetPlannedAmount(record.PlannedAmount, record.PlannedSponsorship, currencyRate.Rate);
        }

        // validate names
        var uniqTitleCount = records.Select(r => r.Title).Distinct().Count();
        if (records.Count != uniqTitleCount)
            throw new ApiException("File contains Grouped Activities with same names");


        var segments =
            await _dictionaryRepository.FindOneAsync(d => d.InternalName == BuiltInDictionariesInternalNames.Segments);
        var campaigns =
            await _dictionaryRepository.FindOneAsync(d => d.InternalName == BuiltInDictionariesInternalNames.Campaigns);

        // get all segments & campaigns from records
        var importSegments = records.SelectMany(r => r.Segments).Select(s => s.Title).Distinct().ToList();
        var importCampaigns = records.SelectMany(r => r.Campaigns).Select(c => c.Title).Distinct().ToList();


        // validate segments & campaigns exist in dictionary
        foreach (var segment in importSegments)
        {
            if (segments.Items.All(s => s != segment))
                throw new ApiException("Segment not found: " + segment);
        }

        foreach (var campaign in importCampaigns)
        {
            if (campaigns.Items.All(c => c != campaign))
                throw new ApiException("Campaign not found: " + campaign);
        }

        // validate that segments & campaigns sum == 100%
        foreach (var record in records)
        {
            var segmentsSum = record.Segments.Sum(s => s.Value);
            var campaignSum = record.Campaigns.Sum(c => c.Value);

            if (Convert.ToInt32(segmentsSum) != 100)
                throw new ApiException("Values sum for segments should be equal to 100%");

            if (Convert.ToInt32(campaignSum) != 100)
                throw new ApiException("Values sum for campaigns should be equal to 100%");
        }

        // remove old grouped activities for current plan
        var toMarkAsDeleted = await GetByBudgetPlanId(budgetPlanId);
        foreach (var gaToDelete in toMarkAsDeleted)
        {
            _groupedActivitiesRepository.Remove(gaToDelete.Id);
        }

        await _groupedActivitiesRepository.CreateManyAsync(records);
    }

    public async Task<BudgetPlanGroupedActivitySummary> GetSummaryReport(string budgetPlanId)
    {
        var groupedActivities = await GetByBudgetPlanId(budgetPlanId);

        var result = new BudgetPlanGroupedActivitySummary
        {
            PlanId = budgetPlanId
        };
        foreach (var ga in groupedActivities)
            switch (ga.Quarter)
            {
                case 1:
                    result.NetPlannedAmountQ1 += ga.NetPlannedAmount;
                    break;
                case 2:
                    result.NetPlannedAmountQ2 += ga.NetPlannedAmount;
                    break;
                case 3:
                    result.NetPlannedAmountQ3 += ga.NetPlannedAmount;
                    break;
                case 4:
                    result.NetPlannedAmountQ4 += ga.NetPlannedAmount;
                    break;
            }

        return result;
    }

    public Task<List<GroupedActivity>> GetAsync()
    {
        return _groupedActivitiesRepository.GetAsync();
    }

    public async Task<GroupedActivity> GetAsync(string id)
    {
        var result = await _groupedActivitiesRepository.GetAsync(id);
        return result;
    }

    public Task RemoveAsync(GroupedActivity ga)
    {
        return _groupedActivitiesRepository.RemoveAsync(ga.Id);
    }

    public Task<GroupedActivity> Clone(GroupedActivity ga, string toPlanId)
    {
        ga.Id = null;
        ga.BudgetPlanId = toPlanId;
        return _groupedActivitiesRepository.CreateAsync(ga);
    }

    public async Task Clone(string fromPlanId, string toPlanId)
    {
        var groupedActivities = await _groupedActivitiesRepository.FindAsync(g => g.BudgetPlanId == fromPlanId);

        foreach (var ga in groupedActivities)
        {
            ga.Id = null;
            ga.BudgetPlanId = toPlanId;
        }

        if (groupedActivities.Count > 0)
        {
            await _groupedActivitiesRepository.CreateManyAsync(groupedActivities);
        }
    }

    public void RemoveOldItems(TimeSpan fromDays)
    {
        var minDate = DateTime.Now.Subtract(fromDays);
        _groupedActivitiesRepository.Remove(ga => ga.IsDeleted
                                                  && ga.Modified < minDate, true);
    }
}
