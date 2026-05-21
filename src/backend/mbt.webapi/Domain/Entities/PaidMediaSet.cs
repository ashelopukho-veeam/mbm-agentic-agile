using System.Collections.Generic;
using mbt.webapi.Domain.Entities.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace mbt.webapi.Domain.Entities;

public class PaidMediaSet : BaseItem, IIsDeleted
{
    public string PaidMediaSetType { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string LinkedItemId { get; set; }

    public string Comment { get; set; }

    public List<PaidMediaDetails> Details { get; set; }

    public bool IsDeleted { get; set; }
}

public static class PaidMediaSetTypes
{
    public const string Transfer = "Transfer";
    public const string IncrementalFund = "IncrementalFund";
    public const string Delta = "Delta";
}
