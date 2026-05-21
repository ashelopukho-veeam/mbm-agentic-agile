using System.Collections.Generic;
using JetBrains.Annotations;
using mbt.webapi.Domain;
using mbt.webapi.Domain.Entities;
using MediatR;

namespace mbt.webapi.UseCases.GA.EditGroupedActivities;

[PublicAPI]
public class EditGroupedActivityToBudgetRequest : IRequest<GroupedActivity>, IWithPaidMediaData
{
    public string Id { get; set; }

    public string Title { get; set; }

    public int Quarter { get; set; }
    public double PlannedAmount { get; set; }
    public double PlannedSponsorship { get; set; }

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
}
