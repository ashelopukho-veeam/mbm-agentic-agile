using System.Collections.Generic;
using mbt.webapi.Domain.Entities.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace mbt.webapi.Domain.Entities;

public class GroupedActivity : BaseItem, IIsDeleted, IWithPaidMediaData
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string BudgetId { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
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

    public bool IsDeleted { get; set; }

    public List<TitleNumberValuePair> Segments { get; set; } = new();
    public List<TitleNumberValuePair> Campaigns { get; set; } = new();
}
