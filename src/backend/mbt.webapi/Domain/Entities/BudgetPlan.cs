using System.Collections.Generic;
using mbt.webapi.Domain.Entities.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace mbt.webapi.Domain.Entities;

[BsonIgnoreExtraElements]
public class BudgetPlan : BaseIdItem
{
    public BudgetPlan()
    {
        Id = ObjectId.GenerateNewId().ToString();
    }

    public string Quarter { get; set; }
    public double Q1 { get; set; }
    public double Q2 { get; set; }
    public double Q3 { get; set; }
    public double Q4 { get; set; }

    public List<TitleNumberValuePair> Segments { get; set; } = new();
    public List<TitleNumberValuePair> Campaigns { get; set; } = new();

    public string Status { get; set; }
}
