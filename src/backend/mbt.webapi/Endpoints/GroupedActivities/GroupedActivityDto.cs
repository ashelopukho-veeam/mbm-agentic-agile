using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using mbt.webapi.Endpoints.BudgetStructure.dto;
using TitleNumberValuePair = mbt.webapi.Domain.Entities.TitleNumberValuePair;
using UserProfile = mbt.webapi.Domain.Entities.UserProfile;


namespace mbt.webapi.Endpoints.GroupedActivities;

[PublicAPI]
public class GroupedActivityDto
{
    public string Id { get; set; }
    public string Title { get; set; }

    public string BudgetId { get; set; }
    public string BudgetPlanId { get; set; }

    public int Quarter { get; set; }
    public double PlannedAmount { get; set; }
    public double PlannedSponsorship { get; set; }
    public double NetPlannedAmount { get; set; }
    public string LocalCurrency { get; set; }
    public string Comment { get; set; }
    public string GlobalRegion { get; set; }

    public string SubRegion { get; set; }
    public string Alliance { get; set; }
    public List<string> Vendors { get; set; }
    public string ChannelDetails { get; set; }
    public string AdService { get; set; }
    public string ExecutionTeam { get; set; }
    public string ContentType { get; set; }
    public string GlobalProgram { get; set; }
    public string Management { get; set; }
    public string Team { get; set; }

    public List<TitleNumberValuePair> Segments { get; set; }
    public List<TitleNumberValuePair> Campaigns { get; set; }

    public string CreatedBy { get; set; }
    public DateTime Created { get; set; }

    public string ModifiedBy { get; set; }
    public DateTime Modified { get; set; }
}

[PublicAPI]
public class GroupedActivityExpandedDto : GroupedActivityDto
{
    public UserProfile CreatedByUser { get; set; }
    public UserProfile ModifiedByUser { get; set; }
    public BudgetStructureDto Budget { get; set; }
}
